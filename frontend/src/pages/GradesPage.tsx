import { useState } from "react";
import type { FormEvent } from "react";
import { gradesApi } from "../api/services";
import { extractErrorMessage } from "../api/client";
import { Alert, Spinner, StatusBadge } from "../components/ui";
import type { EnterGradePayload, GradeResponse, StudentLookup } from "../types";

const TERMS = ["Term 1", "Term 2", "Term 3"];

export function GradesPage() {
  const [registrationNumber, setRegistrationNumber] = useState("");
  const [student, setStudent] = useState<StudentLookup | null>(null);
  const [lookupLoading, setLookupLoading] = useState(false);
  const [lookupError, setLookupError] = useState<string | null>(null);

  const [form, setForm] = useState<Omit<EnterGradePayload, "registrationNumber">>(
    {
      subjectId: 1,
      midtermScore: 0,
      finalScore: 0,
      term: "Term 1",
      academicYear: String(new Date().getFullYear()),
      comments: "",
    }
  );
  const [saving, setSaving] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [result, setResult] = useState<GradeResponse | null>(null);

  const handleLookup = async (e: FormEvent) => {
    e.preventDefault();
    setLookupLoading(true);
    setLookupError(null);
    setStudent(null);
    setResult(null);
    try {
      const s = await gradesApi.lookupStudent(registrationNumber.trim());
      setStudent(s);
    } catch (err) {
      setLookupError(extractErrorMessage(err, "Student not found."));
    } finally {
      setLookupLoading(false);
    }
  };

  const handleEnter = async (e: FormEvent) => {
    e.preventDefault();
    if (!student) return;
    setSaving(true);
    setFormError(null);
    setResult(null);
    try {
      const res = await gradesApi.enterGrade({
        registrationNumber: student.admissionNumber,
        ...form,
        subjectId: Number(form.subjectId),
        midtermScore: Number(form.midtermScore),
        finalScore: Number(form.finalScore),
      });
      setResult(res);
    } catch (err) {
      setFormError(extractErrorMessage(err, "Failed to save grade."));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Enter Grades</h1>
          <p>Look up a student, then record midterm and final scores.</p>
        </div>
      </div>

      <div className="card" style={{ marginBottom: "1.25rem" }}>
        <form className="toolbar" style={{ marginBottom: 0 }} onSubmit={handleLookup}>
          <div className="field" style={{ margin: 0, minWidth: 240 }}>
            <label>Registration / Admission number</label>
            <input
              value={registrationNumber}
              onChange={(e) => setRegistrationNumber(e.target.value)}
              placeholder="e.g. ADM-001"
              required
            />
          </div>
          <button className="btn" type="submit" disabled={lookupLoading}>
            {lookupLoading ? "Searching…" : "Look up"}
          </button>
        </form>
        {lookupError && (
          <div style={{ marginTop: "1rem" }}>
            <Alert kind="error">{lookupError}</Alert>
          </div>
        )}
      </div>

      {lookupLoading && <Spinner />}

      {student && (
        <div className="card">
          <div className="stats-grid" style={{ marginBottom: "1.25rem" }}>
            <div className="stat-card">
              <div className="label">Student</div>
              <div className="value" style={{ fontSize: "1.1rem" }}>
                {student.fullName}
              </div>
            </div>
            <div className="stat-card">
              <div className="label">Class</div>
              <div className="value" style={{ fontSize: "1.1rem" }}>
                {student.className}
              </div>
            </div>
            <div className="stat-card">
              <div className="label">Age</div>
              <div className="value" style={{ fontSize: "1.1rem" }}>
                {student.age}
              </div>
            </div>
          </div>

          <h3>Record grade</h3>
          <p className="muted" style={{ marginBottom: "1rem" }}>
            Midterm and final are each scored out of 50. Subject IDs come from
            the backend's Subjects table.
          </p>

          {formError && <Alert kind="error">{formError}</Alert>}
          {result && (
            <Alert kind="success">
              Saved: {result.subjectName} — total {result.totalScore} (
              {result.status})
            </Alert>
          )}

          <form onSubmit={handleEnter}>
            <div className="row">
              <div className="field">
                <label>Subject ID</label>
                <input
                  type="number"
                  min={1}
                  value={form.subjectId}
                  onChange={(e) =>
                    setForm({ ...form, subjectId: Number(e.target.value) })
                  }
                  required
                />
              </div>
              <div className="field">
                <label>Term</label>
                <select
                  value={form.term}
                  onChange={(e) => setForm({ ...form, term: e.target.value })}
                >
                  {TERMS.map((t) => (
                    <option key={t} value={t}>
                      {t}
                    </option>
                  ))}
                </select>
              </div>
              <div className="field">
                <label>Academic year</label>
                <input
                  value={form.academicYear}
                  onChange={(e) =>
                    setForm({ ...form, academicYear: e.target.value })
                  }
                  required
                />
              </div>
            </div>
            <div className="row">
              <div className="field">
                <label>Midterm score (0–50)</label>
                <input
                  type="number"
                  min={0}
                  max={50}
                  step="0.01"
                  value={form.midtermScore}
                  onChange={(e) =>
                    setForm({ ...form, midtermScore: Number(e.target.value) })
                  }
                  required
                />
              </div>
              <div className="field">
                <label>Final score (0–50)</label>
                <input
                  type="number"
                  min={0}
                  max={50}
                  step="0.01"
                  value={form.finalScore}
                  onChange={(e) =>
                    setForm({ ...form, finalScore: Number(e.target.value) })
                  }
                  required
                />
              </div>
            </div>
            <div className="field">
              <label>Comments (optional)</label>
              <textarea
                rows={2}
                value={form.comments ?? ""}
                onChange={(e) => setForm({ ...form, comments: e.target.value })}
              />
            </div>
            <button className="btn" type="submit" disabled={saving}>
              {saving ? "Saving…" : "Save Grade"}
            </button>
          </form>

          {result && (
            <div style={{ marginTop: "1.5rem" }}>
              <div className="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>Subject</th>
                      <th>Midterm</th>
                      <th>Final</th>
                      <th>Total</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>{result.subjectName}</td>
                      <td>{result.midtermScore}</td>
                      <td>{result.finalScore}</td>
                      <td>{result.totalScore}</td>
                      <td>
                        <StatusBadge status={result.status} />
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
