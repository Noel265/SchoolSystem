import { useCallback, useEffect, useState } from "react";
import type { FormEvent } from "react";
import { subjectsApi } from "../api/services";
import { extractErrorMessage } from "../api/client";
import { Alert, EmptyState, Spinner } from "../components/ui";
import { Modal } from "../components/Modal";
import type { CreateSubjectPayload, Subject } from "../types";

const LEVELS = ["Primary", "Secondary", "Both"];

const emptyForm: CreateSubjectPayload = {
  name: "",
  schoolLevel: "Primary",
};

export function SubjectsPage() {
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<CreateSubjectPayload>(emptyForm);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setSubjects(await subjectsApi.getAll());
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to load subjects."));
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
    setForm(emptyForm);
    setFormError(null);
    setModalOpen(true);
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setFormError(null);
    try {
      await subjectsApi.create(form);
      setModalOpen(false);
      await load();
    } catch (err) {
      setFormError(extractErrorMessage(err, "Failed to save subject."));
    } finally {
      setSaving(false);
    }
  };

  const handleRemove = async (subject: Subject) => {
    if (!window.confirm(`Remove subject "${subject.name}"?`)) return;
    try {
      await subjectsApi.remove(subject.id);
      await load();
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to remove subject."));
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Subjects</h1>
          <p>Manage subjects available for grading.</p>
        </div>
        <button className="btn" onClick={openCreate}>
          + Add Subject
        </button>
      </div>

      {error && <Alert kind="error">{error}</Alert>}

      {loading ? (
        <Spinner />
      ) : subjects.length === 0 ? (
        <EmptyState message="No subjects yet. Add your first subject." />
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>Subject Name</th>
                <th>School Level</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {subjects.map((s, i) => (
                <tr key={s.id}>
                  <td>{i + 1}</td>
                  <td>{s.name}</td>
                  <td>{s.schoolLevel}</td>
                  <td>
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() => handleRemove(s)}
                    >
                      Remove
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {modalOpen && (
        <Modal
          title="Add Subject"
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
              <label>Subject name</label>
              <input
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
                placeholder="e.g. Mathematics"
                required
              />
            </div>
            <div className="field">
              <label>School level</label>
              <select
                value={form.schoolLevel}
                onChange={(e) =>
                  setForm({ ...form, schoolLevel: e.target.value })
                }
              >
                {LEVELS.map((l) => (
                  <option key={l} value={l}>{l}</option>
                ))}
              </select>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}