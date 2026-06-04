import { createContext } from "react";
import type { AuthResponse, Role } from "../types";

export interface AuthUser {
  fullName: string;
  email: string;
  role: Role;
  expiresAt: string;
}

export interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (auth: AuthResponse) => void;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined
);
