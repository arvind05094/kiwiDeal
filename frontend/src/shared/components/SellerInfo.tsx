import { Link } from "react-router-dom";
import { RatingStars } from "./RatingStars";
import { formatDistanceToNow } from "date-fns";

interface SellerInfoProps {
  id: string;
  firstName: string;
  lastName: string;
  memberSince: string;
  averageRating?: number;
  totalRatings: number;
}

export function SellerInfo({
  id,
  firstName,
  lastName,
  memberSince,
  averageRating,
  totalRatings,
}: SellerInfoProps) {
  const initials = `${firstName[0]}${lastName[0]}`.toUpperCase();
  const memberSinceText = formatDistanceToNow(new Date(memberSince), {
    addSuffix: true,
  });

  return (
    <div className="flex items-center gap-3">
      <div className="flex h-10 w-10 items-center justify-center rounded-full bg-gray-200 text-sm font-semibold text-gray-600">
        {initials}
      </div>
      <div>
        <Link
          to={`/users/${id}`}
          className="text-sm font-medium text-gray-900 hover:text-orange-500 transition-colors"
        >
          {firstName} {lastName}
        </Link>
        <div className="flex items-center gap-1.5 mt-0.5">
          {averageRating !== undefined ? (
            <>
              <RatingStars value={averageRating} size="sm" />
              <span className="text-xs text-gray-500">
                {averageRating.toFixed(1)} ({totalRatings})
              </span>
            </>
          ) : (
            <span className="text-xs text-gray-500">No ratings yet</span>
          )}
        </div>
        <p className="text-xs text-gray-400">Member {memberSinceText}</p>
      </div>
    </div>
  );
}
