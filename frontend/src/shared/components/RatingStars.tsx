import { Star } from "lucide-react";

interface RatingStarsProps {
  value: number;
  max?: number;
  interactive?: boolean;
  onChange?: (value: number) => void;
  size?: "sm" | "md";
}

export function RatingStars({
  value,
  max = 5,
  interactive = false,
  onChange,
  size = "sm",
}: RatingStarsProps) {
  const starSize = size === "sm" ? "h-4 w-4" : "h-6 w-6";

  return (
    <div className="flex items-center gap-0.5">
      {Array.from({ length: max }).map((_, i) => (
        <Star
          key={i}
          className={`${starSize} ${
            i < Math.round(value)
              ? "fill-orange-400 text-orange-400"
              : "fill-gray-200 text-gray-200"
          } ${interactive ? "cursor-pointer transition-colors hover:fill-orange-300 hover:text-orange-300" : ""}`}
          onClick={() => interactive && onChange?.(i + 1)}
        />
      ))}
    </div>
  );
}
