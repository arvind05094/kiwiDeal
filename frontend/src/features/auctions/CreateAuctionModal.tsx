import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { X } from "lucide-react";
import { auctionsApi } from "./api";

interface CreateAuctionModalProps {
  listingId: string;
  listingTitle: string;
  onClose: () => void;
  onCreated: (auctionId: string) => void;
}

export function CreateAuctionModal({
  listingId,
  listingTitle,
  onClose,
  onCreated,
}: CreateAuctionModalProps) {
  const queryClient = useQueryClient();
  const [startingPrice, setStartingPrice] = useState("");
  const [startTime, setStartTime] = useState("");
  const [endTime, setEndTime] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});

  const mutation = useMutation({
    mutationFn: () =>
      auctionsApi.createAuction({
        listingId,
        listingTitle,
        startingPrice: parseFloat(startingPrice),
        startTime: new Date(startTime).toISOString(),
        endTime: new Date(endTime).toISOString(),
      }),
    onSuccess: (auctionId) => {
      queryClient.invalidateQueries({ queryKey: ["listing", listingId] });
      onCreated(auctionId);
    },
    onError: (err: any) => {
      const apiErrors = err.response?.data?.errors as
        | Record<string, string[]>
        | undefined;
      if (apiErrors) {
        const flat: Record<string, string> = {};
        Object.entries(apiErrors).forEach(
          ([k, v]) => (flat[k.toLowerCase()] = v[0]),
        );
        setErrors(flat);
      }
    },
  });

  function validate() {
    const e: Record<string, string> = {};
    if (!startingPrice || parseFloat(startingPrice) <= 0)
      e.startingprice = "Starting price must be greater than 0.";
    if (!startTime) e.starttime = "Start time is required.";
    if (!endTime) e.endtime = "End time is required.";
    if (startTime && endTime && new Date(endTime) <= new Date(startTime)) {
      e.endtime = "End time must be after start time.";
    }
    setErrors(e);
    return Object.keys(e).length === 0;
  }

  function handleSubmit() {
    if (validate()) mutation.mutate();
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <Card className="w-full max-w-md mx-4">
        <CardHeader className="flex flex-row items-center justify-between pb-2">
          <CardTitle className="text-base">Create Auction</CardTitle>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600"
          >
            <X className="w-5 h-5" />
          </button>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-sm text-gray-500 truncate">{listingTitle}</p>

          <div className="space-y-1.5">
            <Label htmlFor="startingPrice">Starting price ($)</Label>
            <Input
              id="startingPrice"
              type="number"
              value={startingPrice}
              onChange={(e) => setStartingPrice(e.target.value)}
              placeholder="1.00"
            />
            {errors.startingprice && (
              <p className="text-xs text-red-500">{errors.startingprice}</p>
            )}
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="startTime">Start time</Label>
            <Input
              id="startTime"
              type="datetime-local"
              value={startTime}
              onChange={(e) => setStartTime(e.target.value)}
            />
            {errors.starttime && (
              <p className="text-xs text-red-500">{errors.starttime}</p>
            )}
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="endTime">End time</Label>
            <Input
              id="endTime"
              type="datetime-local"
              value={endTime}
              onChange={(e) => setEndTime(e.target.value)}
            />
            {errors.endtime && (
              <p className="text-xs text-red-500">{errors.endtime}</p>
            )}
          </div>

          <div className="flex gap-2 pt-2">
            <Button variant="outline" className="flex-1" onClick={onClose}>
              Cancel
            </Button>
            <Button
              className="flex-1 bg-[#1a1a1a] hover:bg-gray-800 text-white"
              onClick={handleSubmit}
              disabled={mutation.isPending}
            >
              {mutation.isPending ? "Creating…" : "Create Auction"}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
