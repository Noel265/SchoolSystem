import { useEffect, useState } from "react";
import { attendanceApi, classesApi, studentsApi } from "../api/services";
import { extractErrorMessage } from "../api/client";
import { Alert, EmptyState, Spinner } from "../components/ui";
import type {
  AttendanceStatus,
  SchoolClass,
  StudentAttendanceItem,
  Student,
} from "../types";

const STATUSES: AttendanceStatus[] = ["Present", "Absent", "Late"];

type MarkMap = Record<number, StudentAttendanceItem>;

export function AttendancePage() {
  const [classes, setClasses] = useState<SchoolClass[]>([]);
  const [classId, setClassId] = useState<number>(0);
  const [date, setDate] = useState<string>(
    new Date().toISOString().slice(0, 10)
  );

  const [students, setStudents] = useState<Student[]>([]);
  const [marks, setMarks] = useState<MarkMap>({});

  const [loadingClasses, setLoadingClasses] = useState(true);
  const [loadingStudents, setLoadingStudents] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        const c = await classesApi.getAll();
        setClasses(c);
        if (c.length > 0) setClassId(c[0].id);
      } catch (err) {
        setError(extractErrorMessage(err, "Failed to load classes."));
      } finally {
        setLoadingClasses(false);
      }
    };
    load();
  }, []);

  const loadStudents = async () => {
    if (!classId) return;
    setLoadingStudents(true);
    setError(null);
    setSuccess(null);
    try {
      const list = await studentsApi.getByClass(classId);
      setStudents(list);
      const initial: MarkMap = {};
      // Pre-fill from any existing attendance for this class/date.
      const existing = await attendanceApi
        .getByClassAndDate(classId, date)
        .catch(() => []);
      const byAdmission = new Map(
        existing.map((r) => [r.admissionNumber, r.status])
      );
      for (const s of list) {
        initial[s.id] = {
          studentId: s.id,
          status: byAdmission.get(s.admissionNumber) ?? "Present",
          remarks: "",
        };
      }
      setMarks(initial);
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to load students."));
      setStudents([]);
    } finally {
      setLoadingStudents(false);
    }
  };

  const setStatus = (studentId: number, status: string) => {
    setMarks((prev) => ({
      ...prev,
      [studentId]: { ...prev[studentId], studentId, status },
    }));
  };

  const setRemarks = (studentId: number, remarks: string) => {
    setMarks((prev) => ({
      ...prev,
      [studentId]: { ...prev[studentId], studentId, remarks },
    }));
  };

  const handleSubmit = async () => {
    setSaving(true);
    setError(null);
    setSuccess(null);
    try {
      await attendanceApi.bulkMark({
        classId,
        date,
        attendances: Object.values(marks),
      });
      setSuccess("Attendance saved successfully.");
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to save attendance."));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Attendance</h1>
          <p>Mark daily attendance for a class.</p>
        </div>
      </div>

      {error && <Alert kind="error">{error}</Alert>}
      {success && <Alert kind="success">{success}</Alert>}

      {loadingClasses ? (
        <Spinner />
      ) : (
        <div className="toolbar">
          <div className="field" style={{ margin: 0, minWidth: 200 }}>
            <label>Class</label>
            <select
              value={classId}
              onChange={(e) => setClassId(Number(e.target.value))}
            >
              {classes.length === 0 && <option value={0}>No classes</option>}
              {classes.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </div>
          <div className="field" style={{ margin: 0, minWidth: 180 }}>
            <label>Date</label>
            <input
              type="date"
              value={date}
              onChange={(e) => setDate(e.target.value)}
            />
          </div>
          <button
            className="btn"
            onClick={loadStudents}
            disabled={!classId || loadingStudents}
          >
            {loadingStudents ? "Loading…" : "Load Students"}
          </button>
        </div>
      )}

      {loadingStudents ? (
        <Spinner />
      ) : students.length > 0 ? (
        <>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Admission #</th>
                  <th>Student</th>
                  <th>Status</th>
                  <th>Remarks</th>
                </tr>
              </thead>
              <tbody>
                {students.map((s) => (
                  <tr key={s.id}>
                    <td>{s.admissionNumber}</td>
                    <td>{s.fullName}</td>
                    <td>
                      <select
                        value={marks[s.id]?.status ?? "Present"}
                        onChange={(e) => setStatus(s.id, e.target.value)}
                        style={{ minWidth: 120 }}
                      >
                        {STATUSES.map((st) => (
                          <option key={st} value={st}>
                            {st}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td>
                      <input
                        value={marks[s.id]?.remarks ?? ""}
                        onChange={(e) => setRemarks(s.id, e.target.value)}
                        placeholder="Optional"
                      />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div style={{ marginTop: "1.25rem", textAlign: "right" }}>
            <button className="btn" onClick={handleSubmit} disabled={saving}>
              {saving ? "Saving…" : "Save Attendance"}
            </button>
          </div>
        </>
      ) : (
        <EmptyState message="Select a class and date, then load students to mark attendance." />
      )}
    </div>
  );
}
