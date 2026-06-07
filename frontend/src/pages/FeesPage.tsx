import { useCallback, useEffect, useState } from "react";
import type { FormEvent } from "react";
import { feesApi, studentsApi } from "../api/services";
import { extractErrorMessage } from "../api/client";
import { Alert, EmptyState, Spinner, StatusBadge } from "../components/ui";
import { Modal } from "../components/Modal";
import type {
  CreateFeePayload,
  FeeRecord,
  RecordPaymentPayload,
  Student,
} from "../types";

const TERMS = ["Term 1", "Term 2", "Term 3"];
const FEE_TYPES = ["Tuition", "Exam", "Library", "Sports", "Uniform", "Other"];

const emptyFeeForm: CreateFeePayload = {
  studentId: 0,
  feeType: "Tuition",
  amountDue: 0,
  term: "Term 1",
  academicYear: String(new Date().getFullYear()),
  dueDate: new Date().toISOString().slice(0, 10),
};

export function FeesPage() {
  const [fees, setFees] = useState<FeeRecord[]>([]);
  const [students, setStudents] = useState<Student[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Filters
  const [filterTerm, setFilterTerm] = useState("");
  const [filterYear, setFilterYear] = useState(
    String(new Date().getFullYear())
  );

  // Add fee modal
  const [feeModalOpen, setFeeModalOpen] = useState(false);
  const [feeForm, setFeeForm] = useState<CreateFeePayload>(emptyFeeForm);
  const [feeFormError, setFeeFormError] = useState<string | null>(null);
  const [feeSaving, setFeeSaving] = useState(false);

  // Payment modal
  const [paymentModalOpen, setPaymentModalOpen] = useState(false);
  const [selectedFee, setSelectedFee] = useState<FeeRecord | null>(null);
  const [paymentAmount, setPaymentAmount] = useState(0);
  const [paymentError, setPaymentError] = useState<string | null>(null);
  const [paymentSaving, setPaymentSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [f, s] = await Promise.all([
        feesApi.getAll(filterTerm || undefined, filterYear || undefined),
        studentsApi.getAll().catch(() => []),
      ]);
      setFees(f);
      setStudents(s);
    } catch (err) {
      setError(extractErrorMessage(err, "Failed to load fees."));
    } finally {
      setLoading(false);
    }
  }, [filterTerm, filterYear]);

  useEffect(() => {
    load();
  }, [load]);

  const handleCreateFee = async (e: FormEvent) => {
    e.preventDefault();
    setFeeSaving(true);
    setFeeFormError(null);
    try {
      await feesApi.create(feeForm);
      setFeeModalOpen(false);
      await load();
    } catch (err) {
      setFeeFormError(extractErrorMessage(err, "Failed to create fee."));
    } finally {
      setFeeSaving(false);
    }
  };

  const openPayment = (fee: FeeRecord) => {
    setSelectedFee(fee);
    setPaymentAmount(fee.balance);
    setPaymentError(null);
    setPaymentModalOpen(true);
  };

  const handlePayment = async (e: FormEvent) => {
    e.preventDefault();
    if (!selectedFee) return;
    setPaymentSaving(true);
    setPaymentError(null);
    try {
      const payload: RecordPaymentPayload = {
        feeRecordId: selectedFee.id,
        amountPaid: paymentAmount,
      };
      await feesApi.recordPayment(payload);
      setPaymentModalOpen(false);
      await load();
    } catch (err) {
      setPaymentError(extractErrorMessage(err, "Failed to record payment."));
    } finally {
      setPaymentSaving(false);
    }
  };

  // Summary stats
  const totalDue = fees.reduce((sum, f) => sum + f.amountDue, 0);
  const totalPaid = fees.reduce((sum, f) => sum + f.amountPaid, 0);
  const totalBalance = fees.reduce((sum, f) => sum + f.balance, 0);
  const overdueCount = fees.filter((f) => f.status !== "Paid").length;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Fee Management</h1>
          <p>Track student fees, payments and balances.</p>
        </div>
        <button className="btn" onClick={() => {
          setFeeForm({ ...emptyFeeForm, studentId: students[0]?.id ?? 0 });
          setFeeFormError(null);
          setFeeModalOpen(true);
        }}>
          + Add Fee
        </button>
      </div>

      {error && <Alert kind="error">{error}</Alert>}

      {/* Summary stats */}
      <div className="stats-grid" style={{ marginBottom: "1.25rem" }}>
        <div className="stat-card">
          <div className="label">Total Due</div>
          <div className="value" style={{ fontSize: "1.4rem" }}>
            MK {totalDue.toLocaleString()}
          </div>
        </div>
        <div className="stat-card">
          <div className="label">Total Paid</div>
          <div className="value" style={{ fontSize: "1.4rem", color: "var(--success)" }}>
            MK {totalPaid.toLocaleString()}
          </div>
        </div>
        <div className="stat-card">
          <div className="label">Outstanding Balance</div>
          <div className="value" style={{ fontSize: "1.4rem", color: "var(--danger)" }}>
            MK {totalBalance.toLocaleString()}
          </div>
        </div>
        <div className="stat-card">
          <div className="label">Unpaid Records</div>
          <div className="value" style={{ fontSize: "1.4rem" }}>
            {overdueCount}
          </div>
        </div>
      </div>

      {/* Filters */}
      <div className="toolbar">
        <div className="field" style={{ margin: 0, minWidth: 160 }}>
          <label>Term</label>
          <select
            value={filterTerm}
            onChange={(e) => setFilterTerm(e.target.value)}
          >
            <option value="">All terms</option>
            {TERMS.map((t) => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>
        </div>
        <div className="field" style={{ margin: 0, minWidth: 140 }}>
          <label>Academic year</label>
          <input
            value={filterYear}
            onChange={(e) => setFilterYear(e.target.value)}
          />
        </div>
      </div>

      {loading ? (
        <Spinner />
      ) : fees.length === 0 ? (
        <EmptyState message="No fee records found." />
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Student</th>
                <th>Admission #</th>
                <th>Class</th>
                <th>Fee Type</th>
                <th>Amount Due</th>
                <th>Amount Paid</th>
                <th>Balance</th>
                <th>Term</th>
                <th>Due Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {fees.map((f) => (
                <tr key={f.id}>
                  <td>{f.studentName}</td>
                  <td>{f.admissionNumber}</td>
                  <td>{f.className}</td>
                  <td>{f.feeType}</td>
                  <td>MK {f.amountDue.toLocaleString()}</td>
                  <td>MK {f.amountPaid.toLocaleString()}</td>
                  <td>MK {f.balance.toLocaleString()}</td>
                  <td>{f.term}</td>
                  <td>{f.dueDate}</td>
                  <td><StatusBadge status={f.status} /></td>
                  <td>
                    {f.status !== "Paid" && (
                      <button
                        className="btn btn-secondary btn-sm"
                        onClick={() => openPayment(f)}
                      >
                        Record Payment
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Add Fee Modal */}
      {feeModalOpen && (
        <Modal
          title="Add Fee Record"
          onClose={() => setFeeModalOpen(false)}
          footer={
            <>
              <button
                className="btn btn-secondary"
                onClick={() => setFeeModalOpen(false)}
                disabled={feeSaving}
              >
                Cancel
              </button>
              <button
                className="btn"
                onClick={handleCreateFee}
                disabled={feeSaving}
              >
                {feeSaving ? "Saving…" : "Save"}
              </button>
            </>
          }
        >
          <form onSubmit={handleCreateFee}>
            {feeFormError && <Alert kind="error">{feeFormError}</Alert>}
            <div className="field">
              <label>Student</label>
              <select
                value={feeForm.studentId}
                onChange={(e) =>
                  setFeeForm({ ...feeForm, studentId: Number(e.target.value) })
                }
                required
              >
                <option value={0}>— Select student —</option>
                {students.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.fullName} ({s.admissionNumber})
                  </option>
                ))}
              </select>
            </div>
            <div className="row">
              <div className="field">
                <label>Fee type</label>
                <select
                  value={feeForm.feeType}
                  onChange={(e) =>
                    setFeeForm({ ...feeForm, feeType: e.target.value })
                  }
                >
                  {FEE_TYPES.map((t) => (
                    <option key={t} value={t}>{t}</option>
                  ))}
                </select>
              </div>
              <div className="field">
                <label>Amount due (MK)</label>
                <input
                  type="number"
                  min={1}
                  value={feeForm.amountDue}
                  onChange={(e) =>
                    setFeeForm({
                      ...feeForm,
                      amountDue: Number(e.target.value),
                    })
                  }
                  required
                />
              </div>
            </div>
            <div className="row">
              <div className="field">
                <label>Term</label>
                <select
                  value={feeForm.term}
                  onChange={(e) =>
                    setFeeForm({ ...feeForm, term: e.target.value })
                  }
                >
                  {TERMS.map((t) => (
                    <option key={t} value={t}>{t}</option>
                  ))}
                </select>
              </div>
              <div className="field">
                <label>Academic year</label>
                <input
                  value={feeForm.academicYear}
                  onChange={(e) =>
                    setFeeForm({ ...feeForm, academicYear: e.target.value })
                  }
                  required
                />
              </div>
            </div>
            <div className="field">
              <label>Due date</label>
              <input
                type="date"
                value={feeForm.dueDate}
                onChange={(e) =>
                  setFeeForm({ ...feeForm, dueDate: e.target.value })
                }
                required
              />
            </div>
          </form>
        </Modal>
      )}

      {/* Record Payment Modal */}
      {paymentModalOpen && selectedFee && (
        <Modal
          title="Record Payment"
          onClose={() => setPaymentModalOpen(false)}
          footer={
            <>
              <button
                className="btn btn-secondary"
                onClick={() => setPaymentModalOpen(false)}
                disabled={paymentSaving}
              >
                Cancel
              </button>
              <button
                className="btn"
                onClick={handlePayment}
                disabled={paymentSaving}
              >
                {paymentSaving ? "Saving…" : "Record Payment"}
              </button>
            </>
          }
        >
          <form onSubmit={handlePayment}>
            {paymentError && <Alert kind="error">{paymentError}</Alert>}
            <div className="stats-grid" style={{ marginBottom: "1rem" }}>
              <div className="stat-card">
                <div className="label">Student</div>
                <div className="value" style={{ fontSize: "1rem" }}>
                  {selectedFee.studentName}
                </div>
              </div>
              <div className="stat-card">
                <div className="label">Outstanding Balance</div>
                <div className="value" style={{ fontSize: "1rem", color: "var(--danger)" }}>
                  MK {selectedFee.balance.toLocaleString()}
                </div>
              </div>
            </div>
            <div className="field">
              <label>Amount being paid (MK)</label>
              <input
                type="number"
                min={1}
                max={selectedFee.balance}
                value={paymentAmount}
                onChange={(e) => setPaymentAmount(Number(e.target.value))}
                required
              />
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}