import { useCallback, useMemo, useState } from "react";
import type { ReactNode } from "react";
import { TOKEN_KEY } from "../api/client";
import type { AuthResponse } from "../types";
import { AuthContext } from "./auth-context";
import type { AuthUser } from "./auth-context";

const USER_KEY = "schoolsystem.user";

function clearStoredSession() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

function loadStoredUser(): AuthUser | null {
  const raw = localStorage.getItem(USER_KEY);
  if (!raw) return null;
  try {
    const parsed = JSON.parse(raw) as AuthUser;
    // Drop expired sessions up front so we never start authenticated stale.
    if (new Date(parsed.expiresAt).getTime() < Date.now()) {
      clearStoredSession();
      return null;
    }
    return parsed;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => loadStoredUser());
  const [token, setToken] = useState<string | null>(() =>
    user ? localStorage.getItem(TOKEN_KEY) : null
  );

  const login = useCallback((auth: AuthResponse) => {
    const nextUser: AuthUser = {
      fullName: auth.fullName,
      email: auth.email,
      role: auth.role,
      expiresAt: auth.expiresAt,
    };
    localStorage.setItem(TOKEN_KEY, auth.token);
    localStorage.setItem(USER_KEY, JSON.stringify(nextUser));
    setToken(auth.token);
    setUser(nextUser);
  }, []);

  const logout = useCallback(() => {
    clearStoredSession();
    setToken(null);
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({
      user,
      token,
      isAuthenticated: Boolean(token && user),
      login,
      logout,
    }),
    [user, token, login, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
