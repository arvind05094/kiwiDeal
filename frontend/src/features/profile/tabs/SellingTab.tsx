import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { profileApi } from "../api";
import { PagedList } from "@/shared/components/PagedList";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { StatusBadge } from "@/shared/components/StatusBadge";
import { cn } from "@/lib/utils";
import { format } from "date-fns";
import type { AuctionStatus } from "@/shared/types/common";

type SellingFilter = "Scheduled" | "Active" | "Sold" | "Unsold";

const FILTERS: { label: string; value: SellingFilter }[] = [
  { label: "Scheduled", value: "Scheduled" },
  { label: "Active", value: "Active" },
  { label: "Sold", value: "Sold" },
  { label: "Unsold", value: "Unsold" },
];

export function SellingTab() {
  const [status, setStatus] = useState<SellingFilter>("Active");
  const [page, setPage] = useState(1);

  const { data, isLoading } = useQuery({
    queryKey: ["auctions", "mine", { status, page }],
    queryFn: () =>
      profileApi.getMySelling({ status, pageNumber: page, pageSize: 10 }),
  });

  return (
    <div className="space-y-4">
      <div className="flex gap-1 border-b">
        {FILTERS.map((f) => (
          <button
            key={f.value}
            onClick={() => {
              setStatus(f.value);
              setPage(1);
            }}
            className={cn(
              "px-4 py-2 text-sm font-medium border-b-2 transition-colors",
              status === f.value
                ? "border-orange-500 text-orange-600"
                : "border-transparent text-gray-500 hover:text-gray-800",
            )}
          >
            {f.label}
          </button>
        ))}
      </div>

      {isLoading ? (
        <LoadingSpinner />
      ) : !data?.items.length ? (
        <EmptyState
          title={`No ${status.toLowerCase()} auctions`}
          description="Nothing to show here."
        />
      ) : (
        <>
          <div className="space-y-3">
            {data.items.map((a) => (
              <Link
                key={a.id}
                to={`/auctions/${a.id}`}
                className="flex items-center justify-between bg-white border rounded-lg px-5 py-4 hover:shadow-sm transition-shadow"
              >
                <div>
                  <p className="font-medium text-gray-900">{a.listingTitle}</p>
                  <p className="text-sm text-gray-500 mt-0.5">
                    Ends {format(new Date(a.endTime), "dd MMM yyyy HH:mm")}
                  </p>
                </div>
                <div className="flex items-center gap-4 shrink-0">
                  <div className="text-right">
                    <p className="text-sm text-gray-500">
                      {a.currentHighestBid != null
                        ? `$${a.currentHighestBid.toFixed(2)}`
                        : `$${a.startingPrice.toFixed(2)}`}
                    </p>
                    <p className="text-xs text-gray-400">
                      {a.bids.length} bid{a.bids.length !== 1 ? "s" : ""}
                    </p>
                  </div>
                  <StatusBadge status={a.status} />
                </div>
              </Link>
            ))}
          </div>
          <PagedList
            currentPage={data.pageNumber}
            totalPages={data.totalPages}
            onPageChange={setPage}
          />
        </>
      )}
    </div>
  );
}
