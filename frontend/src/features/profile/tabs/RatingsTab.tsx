import { useQuery } from "@tanstack/react-query";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { getRatingPrompts } from "@/features/ratings/api";
import { RatingPromptCard } from "@/features/ratings/RatingPromptCard";

export function RatingsTab() {
  const { data: prompts = [], isLoading } = useQuery({
    queryKey: ["rating-prompts"],
    queryFn: getRatingPrompts,
  });

  if (isLoading) return <LoadingSpinner />;

  if (!prompts.length) {
    return (
      <EmptyState
        title="No pending ratings"
        message="You have no ratings to submit."
      />
    );
  }

  return (
    <div className="space-y-4 max-w-xl">
      {prompts.map((p) => (
        <RatingPromptCard key={p.id} prompt={p} />
      ))}
    </div>
  );
}
