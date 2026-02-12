import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Star } from "lucide-react";
import { submitRating, dismissRatingPrompt } from "./api";
import type { RatingPromptDto } from "./types";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

interface Props {
  prompt: RatingPromptDto;
}

export function RatingPromptCard({ prompt }: Props) {
  const queryClient = useQueryClient();
  const [stars, setStars] = useState(0);
  const [hovered, setHovered] = useState(0);
  const [comment, setComment] = useState("");
  const [submitted, setSubmitted] = useState(false);

  const submitMutation = useMutation({
    mutationFn: () =>
      submitRating(prompt.ratedUserId, {
        stars,
        comment: comment || undefined,
      }),
    onSuccess: () => {
      setSubmitted(true);
      queryClient.invalidateQueries({ queryKey: ["rating-prompts"] });
    },
  });

  const dismissMutation = useMutation({
    mutationFn: () => dismissRatingPrompt(prompt.id),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["rating-prompts"] }),
  });

  if (submitted) {
    return (
      <div className="bg-white border rounded-lg p-5 text-center text-sm text-green-600 font-medium">
        Rating submitted — thanks!
      </div>
    );
  }

  return (
    <div className="bg-white border rounded-lg p-5 space-y-4">
      <div>
        <p className="text-sm text-gray-500 mb-0.5">
          Rate your {prompt.role === "Buyer" ? "buyer" : "seller"}
        </p>
        <p className="font-medium text-gray-900">{prompt.ratedUserName}</p>
      </div>

      {/* Star selector */}
      <div className="flex gap-1">
        {[1, 2, 3, 4, 5].map((n) => (
          <button
            key={n}
            onMouseEnter={() => setHovered(n)}
            onMouseLeave={() => setHovered(0)}
            onClick={() => setStars(n)}
            className="focus:outline-none"
          >
            <Star
              className={`h-7 w-7 transition-colors ${
                n <= (hovered || stars)
                  ? "fill-orange-400 text-orange-400"
                  : "text-gray-300"
              }`}
            />
          </button>
        ))}
      </div>

      <Textarea
        placeholder="Leave a comment (optional)"
        value={comment}
        onChange={(e) => setComment(e.target.value)}
        rows={3}
      />

      <div className="flex gap-2">
        <Button
          onClick={() => submitMutation.mutate()}
          disabled={stars === 0 || submitMutation.isPending}
          className="bg-gray-900 hover:bg-gray-800 text-white"
        >
          Submit
        </Button>
        <Button
          variant="outline"
          onClick={() => dismissMutation.mutate()}
          disabled={dismissMutation.isPending}
        >
          Skip
        </Button>
      </div>
    </div>
  );
}
