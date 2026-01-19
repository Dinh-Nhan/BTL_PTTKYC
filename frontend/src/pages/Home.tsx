import Header from "@/components/client/Header";
import RoomListing from "@/components/client/RoomListing";
import SearchPanel from "@/components/client/SearchPanel";
import { useState } from "react";

const Home = () => {
  const [searchParams, setSearchParams] = useState({
    checkIn: "",
    checkOut: "",
    guests: 1,
    priceRange: [0, 500],
    roomType: "all",
  });

  return (
    <>
      <main className="min-h-screen bg-background">
        <Header />
        <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          <SearchPanel onSearch={setSearchParams} />
          <RoomListing searchParams={searchParams} />
        </div>
      </main>
    </>
  );
};

export default Home;
