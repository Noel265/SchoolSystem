export function Spinner() {
  return <div className="spinner" role="status" aria-label="Loading" />;
}

export function Alert({
  kind,
  children,
}: {
  kind: "error" | "success";
  children: React.ReactNode;
}) {
  return <div className={`alert alert-${kind}`}>{children}</div>;
}

export function EmptyState({ message }: { message: string }) {
  return <div className="empty">{message}</div>;
}

export function StatusBadge({ status }: { status: string }) {
  const s = status.toLowerCase();
  let cls = "badge-muted";
  if (s === "present" || s === "pass" || s === "passed" || s === "active") {
    cls = "badge-success";
  } else if (s === "absent" || s === "fail" || s === "failed" || s === "inactive") {
    cls = "badge-danger";
  } else if (s === "late") {
    cls = "badge-warning";
  }
  return <span className={`badge ${cls}`}>{status}</span>;
}
