import { priorityLabels, statusLabels } from "@/lib/labels";
import type { Priority, Status } from "@/lib/types";

const base =
  "inline-flex items-center rounded-sm px-2 py-0.5 text-sm font-medium whitespace-nowrap";

const statusStyles: Record<Status, string> = {
  Pending: "bg-pending-soft text-pending",
  InProgress: "bg-progress-soft text-progress",
  Resolved: "bg-resolved-soft text-resolved",
};

const priorityStyles: Record<Priority, string> = {
  Low: "bg-low-soft text-low",
  Medium: "bg-pending-soft text-pending",
  High: "bg-high-soft text-high",
};

export function StatusBadge({ status }: { status: Status }) {
  return <span className={`${base} ${statusStyles[status]}`}>{statusLabels[status]}</span>;
}

export function PriorityBadge({ priority }: { priority: Priority }) {
  return (
    <span className={`${base} ${priorityStyles[priority]}`}>{priorityLabels[priority]}</span>
  );
}
