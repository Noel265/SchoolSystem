import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { classesApi, studentsApi, teachersApi } from "../api/services";
import { useAuth } from "../hooks/useAuth";
import { Spinner } from "../components/ui";

interface Stats {
  students?: number;
  classes?: number;
  teachers?: number;
}

export function DashboardPage() {
  const { user } = useAuth();
  const [stats, setStats] = useState<Stats>({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    const load = async () => {
      setLoading(true);
      const next: Stats = {};
      const canManage = user?.role === "Admin" || user?.role === "Teacher";
      try {
        if (canManage) {
          const [students, classes] = await Promise.all([
            studentsApi.getAll().catch(() => []),
            classesApi.getAll().catch(() => []),
          ]);
          next.students = students.length;
          next.classes = classes.length;
        }
        if (user?.role === "Admin") {
          const teachers = await teachersApi.getAll().catch(() => []);
          next.teachers = teachers.length;
        }
      } finally {
        if (!cancelled) {
          setStats(next);
          setLoading(false);
        }
      }
    };
    load();
    return () => {
      cancelled = true;
    };
  }, [user?.role]);

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Dashboard</h1>
          <p>Overview of your school at a glance.</p>
        </div>
      </div>

      {loading ? (
        <Spinner />
      ) : (
        <>
          <div className="stats-grid">
            {stats.students !== undefined && (
              <div className="stat-card">
                <div className="label">Total Students</div>
                <div className="value">{stats.students}</div>
              </div>
            )}
            {stats.classes !== undefined && (
              <div className="stat-card">
                <div className="label">Classes</div>
                <div className="value">{stats.classes}</div>
              </div>
            )}
            {stats.teachers !== undefined && (
              <div className="stat-card">
                <div className="label">Teachers</div>
                <div className="value">{stats.teachers}</div>
              </div>
            )}
            {user?.role === "Parent" && (
              <div className="stat-card">
                <div className="label">Account</div>
                <div className="value" style={{ fontSize: "1.1rem" }}>
                  Parent Portal
                </div>
              </div>
            )}
          </div>

          <div className="card">
            <h3>Quick actions</h3>
            <p className="muted" style={{ marginBottom: "1rem" }}>
              Jump straight to common tasks.
            </p>
            <div className="row">
              {(user?.role === "Admin" || user?.role === "Teacher") && (
                <>
                  <Link className="btn btn-secondary" to="/students">
                    Manage students
                  </Link>
                  <Link className="btn btn-secondary" to="/attendance">
                    Mark attendance
                  </Link>
                  <Link className="btn btn-secondary" to="/grades">
                    Enter grades
                  </Link>
                </>
              )}
              <Link className="btn btn-secondary" to="/report-cards">
                View report cards
              </Link>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
