import { useState } from "react";
import { OverviewTab } from "./tabs/OverviewTab";
import { ProfileTab } from "./tabs/ProfileTab";
import { MyListingsTab } from "./tabs/MyListingsTab";
import { BuyingTab } from "./tabs/BuyingTab";
import { InboxPage } from "@/features/messages/InboxPage";
import { cn } from "@/lib/utils";

const TABS = [
  { id: "overview", label: "Overview" },
  { id: "profile", label: "Profile" },
  { id: "listings", label: "My Listings" },
  { id: "buying", label: "Buying" },
  { id: "inbox", label: "Inbox" },
] as const;

type TabId = (typeof TABS)[number]["id"];

export function MyAccountPage() {
  const [active, setActive] = useState<TabId>("overview");

  function renderTab() {
    switch (active) {
      case "overview":
        return <OverviewTab />;
      case "profile":
        return <ProfileTab />;
      case "listings":
        return <MyListingsTab />;
      case "buying":
        return <BuyingTab />;
      case "inbox":
        return <InboxPage />;
    }
  }

  return (
    <div className="max-w-6xl mx-auto">
      <h1 className="text-2xl font-semibold text-gray-900 mb-6">My Account</h1>
      <div className="flex gap-1 overflow-x-auto border-b mb-6 pb-0">
        {TABS.map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActive(tab.id)}
            className={cn(
              "px-4 py-2 text-sm font-medium whitespace-nowrap border-b-2 transition-colors",
              active === tab.id
                ? "border-orange-500 text-orange-600"
                : "border-transparent text-gray-500 hover:text-gray-800",
            )}
          >
            {tab.label}
          </button>
        ))}
      </div>
      <div>{renderTab()}</div>
    </div>
  );
}
