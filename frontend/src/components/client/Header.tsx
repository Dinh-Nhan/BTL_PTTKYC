import { Button } from "../ui/button";

const Header = () => {
  return (
    <header className="border-b border-border bg-card">
      <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-light tracking-tight text-foreground">
              Khách sạn Hogwarts
            </h1>
            <p className="mt-1 text-sm text-muted-foreground">
              Hãy lựa chọn chúng tôi nếu bạn muốn có một đêm tuyệt vời
            </p>
          </div>
          <nav className="flex items-center gap-8">
            <a
              href="#"
              className="text-sm text-muted-foreground hover:text-foreground transition-colors"
            >
              My Bookings
            </a>
            <Button>Đăng nhập</Button>
          </nav>
        </div>
      </div>
    </header>
  );
};

export default Header;
