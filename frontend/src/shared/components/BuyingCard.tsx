import { Link } from "react-router-dom";

interface BuyingCardProps {
  listingId: string;
  title: string;
  thumbnailUrl?: string;
  badge: string;
  badgeColor: "orange" | "blue";
  price: string;
  category?: string;
  region?: string;
  sublabel: string;
  statusLabel?: string;
  statusColor?: "orange" | "blue" | "gray";
}

export function BuyingCard({
  listingId,
  title,
  thumbnailUrl,
  badge,
  badgeColor,
  price,
  sublabel,
  statusLabel,
  statusColor = "gray",
}: BuyingCardProps) {
  return (
    <Link
      to={`/listings/${listingId}`}
      className="group flex flex-col overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm transition-shadow hover:shadow-md"
    >
      <div className="h-48 overflow-hidden bg-gray-100">
        {thumbnailUrl ? (
          <img
            src={thumbnailUrl}
            alt={title}
            className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-105"
          />
        ) : (
          <div className="flex h-full items-center justify-center text-sm text-gray-300">
            No image
          </div>
        )}
      </div>

      <div className="flex flex-1 flex-col gap-2 p-4">
        <div className="flex items-start justify-between gap-2">
          <h3 className="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-orange-500 transition-colors">
            {title}
          </h3>
          <span
            className={`shrink-0 rounded-full px-2 py-0.5 text-xs font-medium ${
              badgeColor === "orange"
                ? "bg-orange-100 text-orange-600"
                : "bg-blue-100 text-blue-600"
            }`}
          >
            {badge}
          </span>
        </div>

        <div className="mt-auto space-y-1">
          <div className="flex items-center justify-between gap-2">
            <p className="text-lg font-bold text-gray-900">{price}</p>
            {sublabel && (
              <span className="text-xs text-gray-500">{sublabel}</span>
            )}
          </div>

          {statusLabel && (
            <p
              className={`text-xs font-medium ${
                statusColor === "orange"
                  ? "text-orange-500"
                  : statusColor === "blue"
                    ? "text-blue-600"
                    : "text-gray-400"
              }`}
            >
              {statusLabel}
            </p>
          )}
        </div>
      </div>
    </Link>
  );
}
