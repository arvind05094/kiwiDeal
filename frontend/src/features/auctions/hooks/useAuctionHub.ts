import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

interface BidPlacedEvent {
  auctionId: string;
  bidderId: string;
  amount: number;
  newEndTime: string;
}

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
      .withUrl("http://localhost:5158/hubs/auction", {
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
  }, [auctionId]);

  return connectionRef;
}
