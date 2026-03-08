import type { ListingStatus, AuctionStatus } from "@/shared/types/common";

type Status = ListingStatus | AuctionStatus;

const statusStyles: Record<Status, string> = {
  Active: "bg-green-100 text-green-700",
  Scheduled: "bg-yellow-100 text-yellow-700",
  Sold: "bg-blue-100 text-blue-700",
  Cancelled: "bg-gray-100 text-gray-500",
  Closed: "bg-gray-100 text-gray-500",
  PendingPayment: "bg-orange-100 text-orange-700",
};

interface StatusBadgeProps {
  status: Status;
}

export function StatusBadge({ status }: StatusBadgeProps) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${statusStyles[status]}`}
    >
      {status === "PendingPayment" ? "Pending Payment" : status}
    </span>
  );
}
