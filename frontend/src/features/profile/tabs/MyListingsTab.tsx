import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { profileApi } from "../api";
import { ListingCard } from "@/shared/components/ListingCard";
import { PagedList } from "@/shared/components/PagedList";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { cn } from "@/lib/utils";

type ListingFilter = "Active" | "Scheduled" | "Sold" | "Unsold" | "Cancelled";

const FILTERS: { label: string; value: ListingFilter }[] = [
  { label: "Active", value: "Active" },
  { label: "Sold", value: "Sold" },
  { label: "Cancelled", value: "Cancelled" },
];
export function MyListingsTab() {
  const [status, setStatus] = useState<ListingFilter>("Active");

  const [page, setPage] = useState(1);

  const { data, isLoading } = useQuery({
    queryKey: ["listings", "mine", { status, page }],
    queryFn: () =>
      profileApi.getMyListings({
        statuses: [status],
        pageNumber: page,
        pageSize: 12,
      }),
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
          title={`No ${status.toLowerCase()} listings`}
          message="Nothing to show here."
        />
      ) : (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
            {data.items.map((l) => (
              <ListingCard key={l.id} listing={l} />
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
