import { useCallback, useEffect, useState } from "react";
import type { FormEvent } from "react";
import { classesApi, teachersApi } from "../api/services";
import { extractErrorMessage } from "../api/client";
import { useAuth } from "../hooks/useAuth";
import { Alert, EmptyState, Spinner } from "../components/ui";
import { Modal } from "../components/Modal";
import type { CreateClassPayload, SchoolClass, Teacher } from "../types";

const LEVELS = ["Primary", "Secondary"];

const emptyForm: CreateClassPayload = {
  name: "",
  schoolLevel: "Primary",
  academicYear: String(new Date().getFullYear()),
  classTeacherId: null,
};

export function ClassesPage() {
  const { user } = useAuth();
  const isAdmin = user?.role === "Admin";

  const [classes, setClasses] = useState<SchoolClass[]>([]);
  const [teachers, setTeachers] = useState<Teacher[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<SchoolClass | null>(null);
  const [form, setForm] = useState<CreateClassPayload>(emptyForm);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const c = await classesApi.getAll();
      setClasses(c);
      if (isAdmin) {
        const t = await teachersApi.getAll().catch(() => []);
        setTeachers(t);
      }
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to load classes."));
    } finally {
      setLoading(false);
    }
  }, [isAdmin]);

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

  const openEdit = (cls: SchoolClass) => {
    setEditing(cls);
    const matched = teachers.find((t) => t.fullName === cls.classTeacherName);
    setForm({
      name: cls.name,
      schoolLevel: cls.schoolLevel,
      academicYear: cls.academicYear,
      classTeacherId: matched?.id ?? null,
    });
    setFormError(null);
    setModalOpen(true);
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setFormError(null);
    try {
      const payload: CreateClassPayload = {
        ...form,
        classTeacherId: form.classTeacherId || null,
      };
      if (editing) {
        await classesApi.update(editing.id, payload);
      } else {
        await classesApi.create(payload);
      }
      setModalOpen(false);
      await load();
    } catch (err) {
      setFormError(extractErrorMessage(err, "Failed to save class."));
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (cls: SchoolClass) => {
    if (!window.confirm(`Delete class "${cls.name}"?`)) return;
    try {
      await classesApi.remove(cls.id);
      await load();
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to delete class."));
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Classes</h1>
          <p>Organize students into classes by academic year.</p>
        </div>
        {isAdmin && (
          <button className="btn" onClick={openCreate}>
            + Add Class
          </button>
        )}
      </div>

      {error && <Alert kind="error">{error}</Alert>}

      {loading ? (
        <Spinner />
      ) : classes.length === 0 ? (
        <EmptyState message="No classes yet." />
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Level</th>
                <th>Academic Year</th>
                <th>Class Teacher</th>
                <th>Students</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {classes.map((c) => (
                <tr key={c.id}>
                  <td>{c.name}</td>
                  <td>{c.schoolLevel}</td>
                  <td>{c.academicYear}</td>
                  <td>{c.classTeacherName || "—"}</td>
                  <td>{c.studentCount}</td>
                  {isAdmin && (
                    <td>
                      <div className="row" style={{ gap: "0.4rem" }}>
                        <button
                          className="btn btn-secondary btn-sm"
                          onClick={() => openEdit(c)}
                        >
                          Edit
                        </button>
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => handleDelete(c)}
                        >
                          Delete
                        </button>
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
          title={editing ? "Edit Class" : "Add Class"}
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
              <label>Class name</label>
              <input
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
                placeholder="e.g. Grade 5A"
                required
              />
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
            <div className="field">
              <label>Class teacher (optional)</label>
              <select
                value={form.classTeacherId ?? 0}
                onChange={(e) =>
                  setForm({
                    ...form,
                    classTeacherId: Number(e.target.value) || null,
                  })
                }
              >
                <option value={0}>— None —</option>
                {teachers.map((t) => (
                  <option key={t.id} value={t.id}>
                    {t.fullName}
                  </option>
                ))}
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}
