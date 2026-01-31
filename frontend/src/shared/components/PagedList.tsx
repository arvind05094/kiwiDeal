import type { PagedResult } from "@/shared/types/common";
import { Button } from "@/components/ui/button";
import { ChevronLeft, ChevronRight } from "lucide-react";

interface PagedListProps<T> {
  result: PagedResult<T>;
  onPageChange: (page: number) => void;
  children: React.ReactNode;
}

export function PagedList<T>({
  result,
  onPageChange,
  children,
}: PagedListProps<T>) {
  return (
    <div>
      {children}
      {result.totalPages > 1 && (
        <div className="mt-8 flex items-center justify-center gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => onPageChange(result.pageNumber - 1)}
            disabled={!result.hasPreviousPage}
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <span className="text-sm text-gray-600">
            Page {result.pageNumber} of {result.totalPages}
          </span>
          <Button
            variant="outline"
            size="sm"
            onClick={() => onPageChange(result.pageNumber + 1)}
            disabled={!result.hasNextPage}
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      )}
    </div>
  );
}
