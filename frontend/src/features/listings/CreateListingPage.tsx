import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { listingsApi } from "./api";
import { auctionsApi } from "../auctions/api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { NZ_REGIONS, CATEGORIES } from "@/shared/types/common";
import { Upload, X } from "lucide-react";
import type { ProblemDetails } from "@/shared/types/common";

export function CreateListingPage() {
  const navigate = useNavigate();
  const [listingType, setListingType] = useState<"Auction" | "FixedPrice">(
    "FixedPrice",
  );
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [category, setCategory] = useState("");
  const [region, setRegion] = useState("");
  const [buyNowPrice, setBuyNowPrice] = useState("");
  const [startingPrice, setStartingPrice] = useState("");
  const [startTime, setStartTime] = useState("");
  const [endTime, setEndTime] = useState("");
  const [images, setImages] = useState<File[]>([]);
  const [previews, setPreviews] = useState<string[]>([]);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});

  const createMutation = useMutation({
    mutationFn: async () => {
      const id = await listingsApi.createListing({
        title,
        description,
        listingType,
        buyNowPrice:
          listingType === "FixedPrice" ? Number(buyNowPrice) : undefined,
        category,
        region,
      });
      if (images.length > 0) {
        await listingsApi.uploadImages(id, images);
      }
      if (listingType === "Auction") {
        const auctionId = await auctionsApi.createAuction({
          listingId: id,
          listingTitle: title,
          startingPrice: Number(startingPrice),
          startTime: new Date(startTime).toISOString(),
          endTime: new Date(endTime).toISOString(),
        });
        return { id, auctionId };
      }
      return { id, auctionId: null };
    },
    onSuccess: ({ id, auctionId }) =>
      navigate(auctionId ? `/auctions/${auctionId}` : `/listings/${id}`),
    onError: (err: unknown) => {
      const data = (err as { response?: { data?: ProblemDetails } }).response
        ?.data;
      if (data?.errors) setFieldErrors(data.errors);
    },
  });

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files ?? []);
    const combined = [...images, ...files].slice(0, 3);
    setImages(combined);
    setPreviews(combined.map((f) => URL.createObjectURL(f)));
  };

  const removeImage = (i: number) => {
    const next = images.filter((_, idx) => idx !== i);
    setImages(next);
    setPreviews(next.map((f) => URL.createObjectURL(f)));
  };

  const err = (field: string) => fieldErrors[field]?.[0];

  const validate = (): boolean => {
    const errors: Record<string, string[]> = {};

    if (!title.trim()) errors["Title"] = ["Title is required"];
    if (!description.trim())
      errors["Description"] = ["Description is required"];
    if (!category) errors["Category"] = ["Category is required"];
    if (!region) errors["Region"] = ["Region is required"];

    if (listingType === "FixedPrice") {
      if (!buyNowPrice || Number(buyNowPrice) <= 0)
        errors["BuyNowPrice"] = ["Buy now price must be greater than 0"];
    }

    if (listingType === "Auction") {
      if (!startingPrice || Number(startingPrice) <= 0)
        errors["StartingPrice"] = ["Starting price must be greater than 0"];
      const now = new Date();
      const start = new Date(startTime);
      const end = new Date(endTime);
      if (!startTime || start <= now)
        errors["StartTime"] = ["Start time must be in the future"];
      if (!endTime || end <= start)
        errors["EndTime"] = ["End time must be after start time"];
    }

    if (images.length === 0)
      errors["Images"] = ["At least one image is required"];

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  return (
    <div className="max-w-2xl mx-auto space-y-8">
      <h1 className="text-2xl font-semibold text-gray-900">Post a Listing</h1>

      {/* Listing type toggle */}
      <div className="flex rounded-lg border border-gray-200 overflow-hidden">
        {(["FixedPrice", "Auction"] as const).map((type) => (
          <button
            key={type}
            onClick={() => setListingType(type)}
            className={`flex-1 py-2.5 text-sm font-medium transition-colors ${
              listingType === type
                ? "bg-gray-900 text-white"
                : "bg-white text-gray-600 hover:bg-gray-50"
            }`}
          >
            {type === "FixedPrice" ? "Fixed Price" : "Auction"}
          </button>
        ))}
      </div>

      <div className="space-y-5">
        {/* Title */}
        <div className="space-y-1.5">
          <Label htmlFor="title">Title</Label>
          <Input
            id="title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="What are you selling?"
          />
          {err("Title") && (
            <p className="text-xs text-red-500">{err("Title")}</p>
          )}
        </div>

        {/* Description */}
        <div className="space-y-1.5">
          <Label htmlFor="description">Description</Label>
          <Textarea
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Describe your item..."
            rows={5}
          />
          {err("Description") && (
            <p className="text-xs text-red-500">{err("Description")}</p>
          )}
        </div>

        {/* Category + Region */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div className="space-y-1.5">
            <Label>Category</Label>
            <Select value={category} onValueChange={setCategory}>
              <SelectTrigger>
                <SelectValue placeholder="Select category" />
              </SelectTrigger>
              <SelectContent>
                {CATEGORIES.map((c) => (
                  <SelectItem key={c.value} value={c.value}>
                    {c.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {err("Category") && (
              <p className="text-xs text-red-500">{err("Category")}</p>
            )}
          </div>
          <div className="space-y-1.5">
            <Label>Region</Label>
            <Select value={region} onValueChange={setRegion}>
              <SelectTrigger>
                <SelectValue placeholder="Select region" />
              </SelectTrigger>
              <SelectContent>
                {NZ_REGIONS.map((r) => (
                  <SelectItem key={r.value} value={r.value}>
                    {r.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {err("Region") && (
              <p className="text-xs text-red-500">{err("Region")}</p>
            )}
          </div>
        </div>

        {/* Fixed price fields */}
        {listingType === "FixedPrice" && (
          <div className="space-y-1.5">
            <Label htmlFor="price">Buy Now Price ($)</Label>
            <Input
              id="price"
              type="number"
              min={0}
              value={buyNowPrice}
              onChange={(e) => setBuyNowPrice(e.target.value)}
              placeholder="0.00"
            />
            {err("BuyNowPrice") && (
              <p className="text-xs text-red-500">{err("BuyNowPrice")}</p>
            )}
          </div>
        )}

        {/* Auction fields */}
        {listingType === "Auction" && (
          <div className="space-y-4">
            <div className="space-y-1.5">
              <Label htmlFor="startingPrice">Starting Price ($)</Label>
              <Input
                id="startingPrice"
                type="number"
                min={0}
                value={startingPrice}
                onChange={(e) => setStartingPrice(e.target.value)}
                placeholder="1.00"
              />
              {err("StartingPrice") && (
                <p className="text-xs text-red-500">{err("StartingPrice")}</p>
              )}
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="space-y-1.5">
                <Label htmlFor="startTime">Start Time</Label>
                <Input
                  id="startTime"
                  type="datetime-local"
                  value={startTime}
                  onChange={(e) => setStartTime(e.target.value)}
                />
                {err("StartTime") && (
                  <p className="text-xs text-red-500">{err("StartTime")}</p>
                )}
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="endTime">End Time</Label>
                <Input
                  id="endTime"
                  type="datetime-local"
                  value={endTime}
                  onChange={(e) => setEndTime(e.target.value)}
                />
                {err("EndTime") && (
                  <p className="text-xs text-red-500">{err("EndTime")}</p>
                )}
              </div>
            </div>
          </div>
        )}

        {/* Images */}
        <div className="space-y-2">
          <Label>
            Images{" "}
            <span className="text-gray-400 font-normal">(1–3 required)</span>
          </Label>
          <div className="flex gap-3 flex-wrap">
            {previews.map((src, i) => (
              <div key={i} className="relative">
                <img
                  src={src}
                  alt=""
                  className="h-24 w-24 rounded-lg object-cover border border-gray-200"
                />
                <button
                  onClick={() => removeImage(i)}
                  className="absolute -top-1.5 -right-1.5 bg-white rounded-full border border-gray-200 p-0.5 hover:bg-red-50"
                >
                  <X className="h-3 w-3 text-gray-500" />
                </button>
                {i === 0 && (
                  <span className="absolute bottom-1 left-1 text-[10px] bg-black/50 text-white px-1 rounded">
                    Primary
                  </span>
                )}
              </div>
            ))}
            {images.length < 3 && (
              <label className="flex h-24 w-24 cursor-pointer flex-col items-center justify-center rounded-lg border-2 border-dashed border-gray-300 text-gray-400 hover:border-orange-400 hover:text-orange-400 transition-colors">
                <Upload className="h-5 w-5" />
                <span className="text-xs mt-1">Add</span>
                <input
                  type="file"
                  accept=".jpg,.jpeg,.png,.webp"
                  multiple
                  className="hidden"
                  onChange={handleImageChange}
                />
              </label>
            )}
          </div>
          {err("Images") && (
            <p className="text-xs text-red-500">{err("Images")}</p>
          )}
        </div>
      </div>

      <div className="flex gap-3 pt-2">
        <Button
          className="bg-orange-500 hover:bg-orange-600 text-white flex-1"
          onClick={() => {
            if (!validate()) return;
            createMutation.mutate();
          }}
          disabled={createMutation.isPending}
        >
          {createMutation.isPending ? "Posting..." : "Post Listing"}
        </Button>
        <Button variant="outline" onClick={() => navigate(-1)}>
          Cancel
        </Button>
      </div>
    </div>
  );
}
