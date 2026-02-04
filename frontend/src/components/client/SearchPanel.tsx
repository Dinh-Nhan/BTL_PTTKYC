import { useEffect, useState } from "react";
import { ChevronDown } from "lucide-react";
import { Card } from "../ui/card";
import { Button } from "../ui/button";
import roomApi from "@/api/roomApi.js";
interface SearchPanelProps {
  onSearch: (params : any[]) => void;
}

const SearchPanel = ({ onSearch }: SearchPanelProps) => {
  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");
  const [Adult, setAdult] = useState("1");
  const [children, setChildren] = useState("1");
  const [showAdultDropdown, setShowAdultDropdown] = useState(false);
  const [showChildrenDropdown, setShowChildrenDropdown] = useState(false);

  const performSearch = (checkInDate: string, checkOutDate: string, adults: string, kids: string) => {
    // Chỉ search khi có đủ thông tin cần thiết
    if (!checkInDate || !checkOutDate) {
      return;
    }

    roomApi.searchByDate(checkInDate, checkOutDate, adults, kids)
      .then((response) => {
        const mappedResults = response.data.result.map((room: any) => ({
          id: room.roomId,
          name: room.roomType?.typeName,
          description: room.roomType?.description,
          price: room.roomType?.basePrice,
          guests: room.roomType?.maxAdult,
          type: room.roomType?.typeName,
          image: room.roomType?.imageUrl,
          amenities:
            room.roomType?.amenities?.split(",").map((a: string) => a.trim()) ||
            [],
          roomType: room.roomType,
        }));
        onSearch(mappedResults);
      })
      .catch((error) => console.error(error));
  };

  // useEffect để tự động search khi các giá trị thay đổi
  useEffect(() => {
    performSearch(checkIn, checkOut, Adult, children);
  }, [checkIn, checkOut, Adult, children]);

  const today = new Date().toISOString().split("T")[0];

  const handleCheckInChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedDate = e.target.value;

    if (selectedDate < today) {
      alert("Ngày check-in không được trước ngày hiện tại.");
      return;
    }

    setCheckIn(selectedDate);
  };

  return (
    <Card className="mb-12 border-border bg-card p-6 shadow-sm sm:p-8">
      <div className="space-y-6">
        <h2 className="text-x1 font-light text-foreground">
          Tìm điểm dừng chân tiếp theo của bạn
        </h2>

        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {/* Check in day */}
          <div className="space-y-2">
            <label className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
              Check-in
            </label>
            <input
              type="date"
              value={checkIn}
              min={today}
              onChange={handleCheckInChange}
              className="w-full rounded-md border border-input bg-background px-3 py-2.5 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
            />
          </div>

          {/* Check out day */}
          <div className="space-y-2">
            <label className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
              Check-out
            </label>
            <input
              type="date"
              value={checkOut}
              onChange={(e) => setCheckOut(e.target.value)}
              className="w-full rounded-md border border-input bg-background px-3 py-2.5 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
            />
          </div>

          {/* Guests */}
          <div className="space-y-2">
            <label className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
              Adult
            </label>
            <div className="relative">
              <button
                onClick={() => setShowAdultDropdown(!showAdultDropdown)}
                className="w-full rounded-md border border-input bg-background px-3 py-2.5 text-left text-sm text-foreground hover:bg-secondary focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
              >
                <div className="flex items-center justify-between">
                  <span>
                    {Adult} adult{Adult !== "1" ? "s" : ""}
                  </span>
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                </div>
              </button>
              {showAdultDropdown && (
                <div className="absolute right-0 top-full z-10 mt-1 w-full rounded-md border border-border bg-card shadow-lg">
                  {[1, 2, 3, 4, 5, 6].map((num) => (
                    <button
                      key={num}
                      onClick={() => {
                        setAdult(num.toString());
                        setShowAdultDropdown(false);
                      }}
                      className="w-full px-3 py-2 text-left text-sm hover:bg-secondary"
                    >
                      {num} adult{num > 1 ? "s" : ""}
                    </button>
                  ))}
                </div>
              )}
            </div>
          </div>

          <div className="space-y-2">
            <label className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
              Children
            </label>
            <div className="relative">
              <button
                onClick={() => setShowChildrenDropdown(!showChildrenDropdown)}
                className="w-full rounded-md border border-input bg-background px-3 py-2.5 text-left text-sm text-foreground hover:bg-secondary focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
              >
                <div className="flex items-center justify-between">
                  <span>
                    {children} child{children !== "1" ? "ren" : ""}
                  </span>
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                </div>
              </button>
              {showChildrenDropdown && (
                <div className="absolute right-0 top-full z-10 mt-1 w-full rounded-md border border-border bg-card shadow-lg">
                  {[1, 2, 3, 4, 5, 6].map((num) => (
                    <button
                      key={num}
                      onClick={() => {
                        setChildren(num.toString());
                        setShowChildrenDropdown(false);
                      }}
                      className="w-full px-3 py-2 text-left text-sm hover:bg-secondary"
                    >
                      {num} child{num > 1 ? "ren" : ""}
                    </button>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* Search Button */}
          {/* <div className="flex items-end">
            <Button
              // onClick={handleSearch}
              className="w-full rounded-md bg-accent px-6 py-2.5 font-medium text-accent-foreground hover:bg-accent/90 transition-colors"
            >
              Tìm kiếm
            </Button>
          </div> */}
        </div>
      </div>
    </Card>
  );
};

export default SearchPanel;
