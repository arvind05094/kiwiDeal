import { useEffect, useRef, useCallback } from "react";
import * as signalR from "@microsoft/signalr";
import { getAccessToken } from "@/shared/api/client";

const HUB_URL = (
  import.meta.env.VITE_API_URL ?? "http://localhost:5158/api/v1"
).replace("/api/v1", "");

export interface BidPlacedMessage {
  auctionId: string;
  bidderId: string;
  bidderName: string;
  amount: number;
  newEndTime: string;
}

export interface AuctionClosedMessage {
  auctionId: string;
  winnerId?: string;
  finalAmount?: number;
}

interface UseAuctionHubOptions {
  auctionId: string;
  onBidPlaced: (msg: BidPlacedMessage) => void;
  onAuctionClosed: (msg: AuctionClosedMessage) => void;
}

export function useAuctionHub({
  auctionId,
  onBidPlaced,
  onAuctionClosed,
}: UseAuctionHubOptions) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const onBidPlacedRef = useRef(onBidPlaced);
  const onAuctionClosedRef = useRef(onAuctionClosed);

  useEffect(() => {
    onBidPlacedRef.current = onBidPlaced;
  }, [onBidPlaced]);
  useEffect(() => {
    onAuctionClosedRef.current = onAuctionClosed;
  }, [onAuctionClosed]);

  useEffect(() => {
    const token = getAccessToken();
    const url = token
      ? `${HUB_URL}/hubs/auctions?access_token=${token}`
      : `${HUB_URL}/hubs/auctions`;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(url)
      .withAutomaticReconnect()
      .build();

    connection.on("BidPlaced", (msg: BidPlacedMessage) => {
      onBidPlacedRef.current(msg);
    });

    connection.on("AuctionClosed", (msg: AuctionClosedMessage) => {
      onAuctionClosedRef.current(msg);
    });

    connection
      .start()
      .then(() => connection.invoke("JoinAuction", auctionId))
      .catch((err) => console.error("AuctionHub connect error:", err));

    connectionRef.current = connection;

    return () => {
      connection
        .invoke("LeaveAuction", auctionId)
        .catch(() => {})
        .finally(() => connection.stop());
    };
  }, [auctionId]);
}
