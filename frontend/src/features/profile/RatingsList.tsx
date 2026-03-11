import { useQueries } from "@tanstack/react-query";
import { useState } from "react";
import { profileApi } from "./api";
import { RatingStars } from "@/shared/components/RatingStars";
import { format } from "date-fns";

export function RatingsList({ userId }: { userId: string }) {
  const [pageCount, setPageCount] = useState(1);

  const results = useQueries({
    queries: Array.from({ length: pageCount }, (_, i) => ({
      queryKey: ["ratings", userId, i + 1],
      queryFn: () => profileApi.getUserRatings(userId, i + 1, 5),
    })),
  });

  const allItems = results.flatMap((r) => r.data?.items ?? []);
  const lastResult = results[results.length - 1];
  const isFetching = results.some((r) => r.isFetching);
  const hasNextPage = lastResult?.data?.hasNextPage ?? false;
  const isExpanded = pageCount > 1;

  if (allItems.length === 0) return null;

  return (
    <div className="border-t pt-3 space-y-3">
      {allItems.map((r, i) => (
        <div key={i} className="space-y-0.5">
          <div className="flex items-center gap-2 text-sm">
            <RatingStars value={r.stars} size="sm" />
            <span className="font-medium text-gray-800">{r.raterName}</span>
            <span className="text-gray-400">·</span>
            <span className="text-gray-400">
              {format(new Date(r.createdAt), "d MMM yyyy")}
            </span>
          </div>
          {r.comment && (
            <p className="text-sm text-gray-600 italic">"{r.comment}"</p>
          )}
        </div>
      ))}
      <div className="flex gap-4">
        {hasNextPage && (
          <button
            onClick={() => setPageCount((p) => p + 1)}
            disabled={isFetching}
            className="text-sm text-orange-500 hover:text-orange-600 font-medium disabled:opacity-50"
          >
            {isFetching ? "Loading..." : "Show more"}
          </button>
        )}
        {isExpanded && (
          <button
            onClick={() => setPageCount(1)}
            className="text-sm text-gray-400 hover:text-gray-600 font-medium"
          >
            Show less
          </button>
        )}
      </div>
    </div>
  );
}
