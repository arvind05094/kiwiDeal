import { useState } from "react";
import type { ListingImageDto } from "@/shared/types/common";

interface ImageGalleryProps {
  images: ListingImageDto[];
  title: string;
}

export function ImageGallery({ images, title }: ImageGalleryProps) {
  const [selected, setSelected] = useState(0);

  if (images.length === 0) {
    return (
      <div className="flex h-80 items-center justify-center rounded-lg bg-gray-100 text-gray-400">
        No images
      </div>
    );
  }

  return (
    <div className="space-y-3">
      <div className="overflow-hidden rounded-lg bg-gray-100">
        <img
          src={images[selected].url}
          alt={title}
          className="h-80 w-full object-contain"
        />
      </div>
      {images.length > 1 && (
        <div className="flex gap-2">
          {images.map((img, i) => (
            <button
              key={i}
              onClick={() => setSelected(i)}
              className={`overflow-hidden rounded-md border-2 transition-colors ${
                i === selected ? "border-orange-500" : "border-transparent"
              }`}
            >
              <img
                src={img.url}
                alt={`${title} ${i + 1}`}
                className="h-16 w-16 object-cover"
              />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
