import { useCallback, useEffect, useState } from "react";
import type { FormEvent } from "react";
import { teachersApi } from "../api/services";
import { extractErrorMessage } from "../api/client";
import { Alert, EmptyState, Spinner, StatusBadge } from "../components/ui";
import { Modal } from "../components/Modal";
import type { CreateTeacherPayload, Teacher } from "../types";

const emptyForm: CreateTeacherPayload = {
  fullName: "",
  employeeNumber: "",
  phoneNumber: "",
  specialization: "",
  hireDate: new Date().toISOString().slice(0, 10),
  email: "",
  password: "",
};

export function TeachersPage() {
  const [teachers, setTeachers] = useState<Teacher[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Teacher | null>(null);
  const [form, setForm] = useState<CreateTeacherPayload>(emptyForm);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setTeachers(await teachersApi.getAll());
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to load teachers."));
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
    setForm(emptyForm);
    setFormError(null);
    setModalOpen(true);
  };

  const openEdit = (teacher: Teacher) => {
    setEditing(teacher);
    setForm({
      fullName: teacher.fullName,
      employeeNumber: teacher.employeeNumber,
      phoneNumber: teacher.phoneNumber,
      specialization: teacher.specialization,
      hireDate: teacher.hireDate,
      email: teacher.email,
      password: "",
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
        await teachersApi.update(editing.id, {
          fullName: form.fullName,
          phoneNumber: form.phoneNumber,
          specialization: form.specialization,
        });
      } else {
        await teachersApi.create(form);
      }
      setModalOpen(false);
      await load();
    } catch (err) {
      setFormError(extractErrorMessage(err, "Failed to save teacher."));
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async (teacher: Teacher) => {
    if (!window.confirm(`Deactivate ${teacher.fullName}?`)) return;
    try {
      await teachersApi.deactivate(teacher.id);
      await load();
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to deactivate teacher."));
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Teachers</h1>
          <p>Manage teaching staff accounts.</p>
        </div>
        <button className="btn" onClick={openCreate}>
          + Add Teacher
        </button>
      </div>

      {error && <Alert kind="error">{error}</Alert>}

      {loading ? (
        <Spinner />
      ) : teachers.length === 0 ? (
        <EmptyState message="No teachers yet." />
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Employee #</th>
                <th>Full Name</th>
                <th>Email</th>
                <th>Phone</th>
                <th>Specialization</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {teachers.map((t) => (
                <tr key={t.id}>
                  <td>{t.employeeNumber}</td>
                  <td>{t.fullName}</td>
                  <td>{t.email}</td>
                  <td>{t.phoneNumber || "—"}</td>
                  <td>{t.specialization || "—"}</td>
                  <td>
                    <StatusBadge status={t.isActive ? "Active" : "Inactive"} />
                  </td>
                  <td>
                    <div className="row" style={{ gap: "0.4rem" }}>
                      <button
                        className="btn btn-secondary btn-sm"
                        onClick={() => openEdit(t)}
                      >
                        Edit
                      </button>
                      {t.isActive && (
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => handleDeactivate(t)}
                        >
                          Deactivate
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {modalOpen && (
        <Modal
          title={editing ? "Edit Teacher" : "Add Teacher"}
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
            <div className="row">
              <div className="field">
                <label>Employee number</label>
                <input
                  value={form.employeeNumber}
                  onChange={(e) =>
                    setForm({ ...form, employeeNumber: e.target.value })
                  }
                  disabled={Boolean(editing)}
                  required
                />
              </div>
              <div className="field">
                <label>Phone</label>
                <input
                  value={form.phoneNumber}
                  onChange={(e) =>
                    setForm({ ...form, phoneNumber: e.target.value })
                  }
                />
              </div>
            </div>
            <div className="field">
              <label>Specialization</label>
              <input
                value={form.specialization}
                onChange={(e) =>
                  setForm({ ...form, specialization: e.target.value })
                }
                placeholder="e.g. Mathematics"
              />
            </div>
            {!editing && (
              <>
                <div className="field">
                  <label>Hire date</label>
                  <input
                    type="date"
                    value={form.hireDate}
                    onChange={(e) =>
                      setForm({ ...form, hireDate: e.target.value })
                    }
                    required
                  />
                </div>
                <div className="field">
                  <label>Email</label>
                  <input
                    type="email"
                    value={form.email}
                    onChange={(e) =>
                      setForm({ ...form, email: e.target.value })
                    }
                    required
                  />
                </div>
                <div className="field">
                  <label>Password</label>
                  <input
                    type="password"
                    value={form.password}
                    onChange={(e) =>
                      setForm({ ...form, password: e.target.value })
                    }
                    required
                  />
                </div>
              </>
            )}
          </form>
        </Modal>
      )}
    </div>
  );
}
