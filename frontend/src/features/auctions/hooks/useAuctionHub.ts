import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { BidPlacedEvent } from "../types";

interface UseAuctionHubOptions {
  auctionId: string;
  onBidPlaced: (event: BidPlacedEvent) => void;
}

export function useAuctionHub({
  auctionId,
  onBidPlaced,
}: UseAuctionHubOptions) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_URL}/hubs/auction`, {
        accessTokenFactory: () => localStorage.getItem("access_token") ?? "",
      })
      .withAutomaticReconnect()
      .build();

    connection.on("BidPlaced", onBidPlaced);

    connection
      .start()
      .then(() => connection.invoke("JoinAuction", auctionId))
      .catch((err) => console.error("SignalR connection error:", err));

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [auctionId, onBidPlaced]);

  return connectionRef;
}
