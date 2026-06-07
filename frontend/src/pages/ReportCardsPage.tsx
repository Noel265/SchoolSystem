import { useState } from "react";
import type { FormEvent } from "react";
import { gradesApi } from "../api/services";
import { extractErrorMessage } from "../api/client";
import { Alert, Spinner, StatusBadge } from "../components/ui";
import type { ReportCard } from "../types";

const TERMS = ["Term 1", "Term 2", "Term 3"];

export function ReportCardsPage() {
  const [registrationNumber, setRegistrationNumber] = useState("");
  const [term, setTerm] = useState("Term 1");
  const [academicYear, setAcademicYear] = useState(
    String(new Date().getFullYear())
  );
  const [report, setReport] = useState<ReportCard | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setReport(null);
    try {
      const rc = await gradesApi.getReportCard(
        registrationNumber.trim(),
        term,
        academicYear
      );
      setReport(rc);
    } catch (err) {
      setError(extractErrorMessage(err, "No report card found."));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Report Cards</h1>
          <p>Generate a term report card for any student.</p>
        </div>
      </div>

      <div className="card" style={{ marginBottom: "1.25rem" }}>
        <form className="toolbar" style={{ marginBottom: 0 }} onSubmit={handleSubmit}>
          <div className="field" style={{ margin: 0, minWidth: 220 }}>
            <label>Registration / Admission number</label>
            <input
              value={registrationNumber}
              onChange={(e) => setRegistrationNumber(e.target.value)}
              placeholder="e.g. ADM-001"
              required
            />
          </div>
          <div className="field" style={{ margin: 0, minWidth: 140 }}>
            <label>Term</label>
            <select value={term} onChange={(e) => setTerm(e.target.value)}>
              {TERMS.map((t) => (
                <option key={t} value={t}>
                  {t}
                </option>
              ))}
            </select>
          </div>
          <div className="field" style={{ margin: 0, minWidth: 140 }}>
            <label>Academic year</label>
            <input
              value={academicYear}
              onChange={(e) => setAcademicYear(e.target.value)}
              required
            />
          </div>
          <button className="btn" type="submit" disabled={loading}>
            {loading ? "Generating…" : "Generate"}
          </button>
        </form>
      </div>

      {error && <Alert kind="error">{error}</Alert>}
      {loading && <Spinner />}

      {report && (
        <div className="card">
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              flexWrap: "wrap",
              gap: "1rem",
              marginBottom: "1.25rem",
            }}
          >
            <div>
              <h2 style={{ marginBottom: "0.25rem" }}>{report.studentName}</h2>
              <div className="muted">
                {report.registrationNumber} · {report.className} · Age{" "}
                {report.age}
              </div>
              <div className="muted">
                {report.term} · {report.academicYear}
              </div>
            </div>
            <div style={{ textAlign: "right" }}>
              <div className="muted">Overall</div>
              <div style={{ fontSize: "1.8rem", fontWeight: 700 }}>
                {report.overallAverage}
              </div>
              <StatusBadge status={report.overallStatus} />
            </div>
          </div>

          <div className="stats-grid" style={{ marginBottom: "1.25rem" }}>
            <div className="stat-card">
              <div className="label">Midterm Total</div>
              <div className="value" style={{ fontSize: "1.4rem" }}>
                {report.midtermTotal}
              </div>
            </div>
            <div className="stat-card">
              <div className="label">Final Total</div>
              <div className="value" style={{ fontSize: "1.4rem" }}>
                {report.finalTotal}
              </div>
            </div>
            <div className="stat-card">
              <div className="label">Position</div>
              <div className="value" style={{ fontSize: "1.4rem" }}>
                {report.position} / {report.totalStudentsInClass}
              </div>
            </div>
          </div>

          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Subject</th>
                  <th>Midterm</th>
                  <th>Final</th>
                  <th>Total</th>
                  <th>Class Avg</th>
                  <th>Status</th>
                  <th>Teacher</th>
                  <th>Comments</th>
                </tr>
              </thead>
              <tbody>
                {report.subjects.map((sub, i) => (
                  <tr key={`${sub.subjectName}-${i}`}>
                    <td>{sub.subjectName}</td>
                    <td>{sub.midtermScore}</td>
                    <td>{sub.finalScore}</td>
                    <td>{sub.subjectTotal}</td>
                    <td>{sub.classSubjectAverage}</td>
                    <td>
                      <StatusBadge status={sub.status} />
                    </td>
                    <td>{sub.teacherName || "—"}</td>
                    <td>{sub.comments || "—"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
