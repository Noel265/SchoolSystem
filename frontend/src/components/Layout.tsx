import { useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import type { Role } from "../types";

interface NavItem {
  to: string;
  label: string;
  icon: string;
  roles?: Role[];
}

const NAV_ITEMS: NavItem[] = [
  { to: "/", label: "Dashboard", icon: "▦" },
  { to: "/students", label: "Students", icon: "🎓", roles: ["Admin", "Teacher"] },
  { to: "/classes", label: "Classes", icon: "🏫", roles: ["Admin", "Teacher"] },
  { to: "/teachers", label: "Teachers", icon: "👩‍🏫", roles: ["Admin"] },
  {
    to: "/attendance",
    label: "Attendance",
    icon: "🗓",
    roles: ["Admin", "Teacher"],
  },
  { to: "/grades", label: "Grades", icon: "📝", roles: ["Admin", "Teacher"] },
  {
    to: "/report-cards",
    label: "Report Cards",
    icon: "📋",
    roles: ["Admin", "Teacher", "Parent"],
  },
];

export function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  const visibleItems = NAV_ITEMS.filter(
    (item) => !item.roles || (user && item.roles.includes(user.role))
  );

  return (
    <div className="app-shell">
      <aside className={`sidebar ${open ? "open" : ""}`}>
        <div className="brand">
          <span>🏛</span> SchoolSystem
        </div>
        <nav>
          {visibleItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.to === "/"}
              className={({ isActive }) =>
                `nav-link ${isActive ? "active" : ""}`
              }
              onClick={() => setOpen(false)}
            >
              <span className="nav-icon">{item.icon}</span>
              {item.label}
            </NavLink>
          ))}
        </nav>
        <div className="sidebar-footer">
          <div className="sidebar-user">
            <strong>{user?.fullName}</strong>
            {user?.role}
          </div>
          <button
            className="btn btn-secondary btn-sm"
            style={{ width: "100%" }}
            onClick={handleLogout}
          >
            Log out
          </button>
        </div>
      </aside>

      <div className="main">
        <header className="topbar">
          <button
            className="btn btn-secondary btn-sm"
            style={{ display: "none" }}
            onClick={() => setOpen((v) => !v)}
          >
            ☰
          </button>
          <div className="muted" style={{ fontSize: "0.9rem" }}>
            Welcome back, <strong>{user?.fullName}</strong>
          </div>
          <span className="badge badge-muted">{user?.role}</span>
        </header>
        <main className="content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
