import { useCallback, useEffect, useState } from "react";
import type { FormEvent } from "react";
import { classesApi, studentsApi } from "../api/services";
import { extractErrorMessage } from "../api/client";
import { useAuth } from "../hooks/useAuth";
import { Alert, EmptyState, Spinner, StatusBadge } from "../components/ui";
import { Modal } from "../components/Modal";
import type {
  CreateStudentPayload,
  SchoolClass,
  Student,
} from "../types";

const LEVELS = ["Primary", "Secondary"];
const GENDERS = ["Male", "Female"];

const emptyForm: CreateStudentPayload = {
  fullName: "",
  admissionNumber: "",
  dateOfBirth: "",
  gender: "Male",
  schoolLevel: "Primary",
  classId: 0,
};

export function StudentsPage() {
  const { user } = useAuth();
  const isAdmin = user?.role === "Admin";

  const [students, setStudents] = useState<Student[]>([]);
  const [classes, setClasses] = useState<SchoolClass[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Student | null>(null);
  const [form, setForm] = useState<CreateStudentPayload>(emptyForm);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [s, c] = await Promise.all([
        studentsApi.getAll(),
        classesApi.getAll().catch(() => []),
      ]);
      setStudents(s);
      setClasses(c);
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to load students."));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    const run = async () => {
      await load();
    };
    run();
  }, [load]);

  const openCreate = () => {
    setEditing(null);
    setForm({ ...emptyForm, classId: classes[0]?.id ?? 0 });
    setFormError(null);
    setModalOpen(true);
  };

  const openEdit = (student: Student) => {
    setEditing(student);
    const matchedClass = classes.find((c) => c.name === student.className);
    setForm({
      fullName: student.fullName,
      admissionNumber: student.admissionNumber,
      dateOfBirth: student.dateOfBirth,
      gender: student.gender,
      schoolLevel: student.schoolLevel,
      classId: matchedClass?.id ?? 0,
    });
    setFormError(null);
    setModalOpen(true);
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setFormError(null);
    try {
      if (editing) {
        await studentsApi.update(editing.id, {
          fullName: form.fullName,
          dateOfBirth: form.dateOfBirth,
          gender: form.gender,
          schoolLevel: form.schoolLevel,
          classId: form.classId,
        });
      } else {
        await studentsApi.create(form);
      }
      setModalOpen(false);
      await load();
    } catch (err) {
      setFormError(extractErrorMessage(err, "Failed to save student."));
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async (student: Student) => {
    if (!window.confirm(`Deactivate ${student.fullName}?`)) return;
    try {
      await studentsApi.deactivate(student.id);
      await load();
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to deactivate student."));
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Students</h1>
          <p>Manage student enrollment records.</p>
        </div>
        {isAdmin && (
          <button className="btn" onClick={openCreate}>
            + Add Student
          </button>
        )}
      </div>

      {error && <Alert kind="error">{error}</Alert>}

      {loading ? (
        <Spinner />
      ) : students.length === 0 ? (
        <EmptyState message="No students yet." />
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Admission #</th>
                <th>Full Name</th>
                <th>Gender</th>
                <th>Level</th>
                <th>Class</th>
                <th>Status</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {students.map((s) => (
                <tr key={s.id}>
                  <td>{s.admissionNumber}</td>
                  <td>{s.fullName}</td>
                  <td>{s.gender}</td>
                  <td>{s.schoolLevel}</td>
                  <td>{s.className || "—"}</td>
                  <td>
                    <StatusBadge status={s.isActive ? "Active" : "Inactive"} />
                  </td>
                  {isAdmin && (
                    <td>
                      <div className="row" style={{ gap: "0.4rem" }}>
                        <button
                          className="btn btn-secondary btn-sm"
                          onClick={() => openEdit(s)}
                        >
                          Edit
                        </button>
                        {s.isActive && (
                          <button
                            className="btn btn-danger btn-sm"
                            onClick={() => handleDeactivate(s)}
                          >
                            Deactivate
                          </button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {modalOpen && (
        <Modal
          title={editing ? "Edit Student" : "Add Student"}
          onClose={() => setModalOpen(false)}
          footer={
            <>
              <button
                className="btn btn-secondary"
                onClick={() => setModalOpen(false)}
                disabled={saving}
              >
                Cancel
              </button>
              <button className="btn" onClick={handleSubmit} disabled={saving}>
                {saving ? "Saving…" : "Save"}
              </button>
            </>
          }
        >
          <form onSubmit={handleSubmit}>
            {formError && <Alert kind="error">{formError}</Alert>}
            <div className="field">
              <label>Full name</label>
              <input
                value={form.fullName}
                onChange={(e) => setForm({ ...form, fullName: e.target.value })}
                required
              />
            </div>
            <div className="field">
              <label>Admission number</label>
              <input
                value={form.admissionNumber}
                onChange={(e) =>
                  setForm({ ...form, admissionNumber: e.target.value })
                }
                disabled={Boolean(editing)}
                required
              />
            </div>
            <div className="row">
              <div className="field">
                <label>Date of birth</label>
                <input
                  type="date"
                  value={form.dateOfBirth}
                  onChange={(e) =>
                    setForm({ ...form, dateOfBirth: e.target.value })
                  }
                  required
                />
              </div>
              <div className="field">
                <label>Gender</label>
                <select
                  value={form.gender}
                  onChange={(e) => setForm({ ...form, gender: e.target.value })}
                >
                  {GENDERS.map((g) => (
                    <option key={g} value={g}>
                      {g}
                    </option>
                  ))}
                </select>
              </div>
            </div>
            <div className="row">
              <div className="field">
                <label>School level</label>
                <select
                  value={form.schoolLevel}
                  onChange={(e) =>
                    setForm({ ...form, schoolLevel: e.target.value })
                  }
                >
                  {LEVELS.map((l) => (
                    <option key={l} value={l}>
                      {l}
                    </option>
                  ))}
                </select>
              </div>
              <div className="field">
                <label>Class</label>
                <select
                  value={form.classId}
                  onChange={(e) =>
                    setForm({ ...form, classId: Number(e.target.value) })
                  }
                >
                  <option value={0}>— Select —</option>
                  {classes.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}
