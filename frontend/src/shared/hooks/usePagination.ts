import { useState } from "react";

export function usePagination(initialPageSize = 20) {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(initialPageSize);

  const goToPage = (page: number) => setPageNumber(page);
  const nextPage = () => setPageNumber((p) => p + 1);
  const prevPage = () => setPageNumber((p) => Math.max(1, p - 1));
  const reset = () => setPageNumber(1);

  return { pageNumber, pageSize, goToPage, nextPage, prevPage, reset };
}
