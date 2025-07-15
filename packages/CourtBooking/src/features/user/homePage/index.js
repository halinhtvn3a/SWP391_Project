import { memo, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Modal from "react-modal";
import "react-multi-carousel/lib/styles.css";
import { AiOutlineSearch } from "react-icons/ai";
import "./style.scss";
import { fetchPrice } from "api/priceApi";
import { animateScroll as scroll } from "react-scroll";
import SlideShowHomePage from "./SlideShow/SlideShow";
import getUserLocation from "map/Geolocation";
import { jwtDecode } from "jwt-decode";
import axios from "axios";

Modal.setAppElement("#root"); // Add this to avoid screen readers issues

const HomePage = () => {
  const [district, setDistrict] = useState("");
  const [city, setCity] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [branches, setBranches] = useState([]);
  const [prices, setPrices] = useState({});
  const [numberOfCourts, setNumberOfCourts] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [modalIsOpen, setModalIsOpen] = useState(false);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [totalBranches, setTotalBranches] = useState(0);
  const navigate = useNavigate();
  const [isUserVip, setUserVip] = useState(false);
  const [userId, setUserId] = useState(null);
  const [userData, setUserData] = useState(null);
  const [user, setUser] = useState(null);

  const cityDistricts = {
    "Hồ Chí Minh": ["Quận 1", "Quận 2", "Bình Thạnh", "Bình Tân"],
    "Hà Nội": ["Hai Bà Trưng", "Ba Đình", "Đống Đa", "Hoàn Kiếm"],
    "Đà Nẵng": ["Cẩm Lệ", "Hải Châu", "Liên Chiểu", "Ngũ Hành Sơn"],
  };

  useEffect(() => {
    const fetchBranches = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await fetch(
          `https://courtcaller.azurewebsites.net/api/Branches?pageNumber=${pageNumber}&pageSize=${itemsPerPage}`
        );
        const data = await response.json();
        setBranches(data.data);
        setTotalBranches(data.total);
        await fetchPrices(isUserVip, data.data);
        await fetchNumberOfCourts(data.data);
      } catch (err) {
        setError("Failed to fetch data");
      } finally {
        setLoading(false);
      }
    };

    fetchBranches();
  }, [pageNumber]);

  const itemsPerPage = 9;
  const totalPages = Math.ceil(totalBranches / itemsPerPage);

  const isJson = (str) => {
    try {
      JSON.parse(str);
    } catch (error) {
      return false;
    }
    return true;
  };

  console.log("branches", branches);

  const fetchNumberOfCourts = async (branchData) => {
    const courtsData = {};
    for (const branch of branchData) {
      try {
        const response = await fetch(
          `https://courtcaller.azurewebsites.net/numberOfCourt/${branch.branchId}`
        );
        const data = await response.json();
        courtsData[branch.branchId] = data;
      } catch (err) {
        console.error(
          `Failed to fetch number of courts for branch ${branch.branchId}`
        );
      }
    }
    setNumberOfCourts(courtsData);
  };

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      const decodedToken = jwtDecode(token);
      setUserId(decodedToken.Id);

      const fetchUserData = async (id, isGoogle) => {
        try {
          if (isGoogle) {
            const response = await axios.get(
              `https://courtcaller.azurewebsites.net/api/UserDetails/GetUserDetailByUserEmail/${id}`
            );
            setUserData(response.data);
            setUserVip(response.data.isVip);

            const userResponse = await axios.get(
              `https://courtcaller.azurewebsites.net/api/Users/GetUserDetailByUserEmail/${id}?searchValue=${id}`
            );
            setUser(userResponse.data);
          } else {
            const response = await axios.get(
              `https://courtcaller.azurewebsites.net/api/UserDetails/${id}`
            );
            setUserData(response.data);
            console.log("response nè:", response.data.isVip);
            setUserVip(response.data.isVip);
            const userResponse = await axios.get(
              `https://courtcaller.azurewebsites.net/api/Users/${id}`
            );
            setUser(userResponse.data);
          }
        } catch (error) {
          console.error("Error fetching user data:", error);
        }
      };

      if (decodedToken.iss !== "https://accounts.google.com") {
        const userId = decodedToken.Id;
        setUserId(userId);
        fetchUserData(userId, false);
      } else {
        const userId = decodedToken.email;
        setUserId(userId);
        fetchUserData(userId, true);
      }
    }
  }, []);

  const fetchPrices = async (isUserVip, branchData) => {
    const pricesData = {};
    for (const branch of branchData) {
      try {
        const { weekdayPrice, weekendPrice } = await fetchPrice(
          isUserVip,
          branch.branchId
        );
        pricesData[branch.branchId] = { weekdayPrice, weekendPrice };
      } catch (err) {
        console.error(`Failed to fetch price for branch ${branch.branchId}`);
      }
    }
    setPrices(pricesData);
  };

  const handleSearch = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(
        `https://courtcaller.azurewebsites.net/api/Branches?pageNumber=${pageNumber}&pageSize=${itemsPerPage}&searchQuery=${district + ", " + city}`
      );
      const data = await response.json();
      setBranches(data.data); 
      setTotalBranches(data.total);
      await fetchPrices(data.data);
      await fetchNumberOfCourts(data.data);
    } catch (err) {
      setError("Failed to fetch data");
    } finally {
      setLoading(false);
    }
  };

  const handleSortByDistance = async () => {
    setLoading(true);
    setError(null);
    try {
      let position = await getUserLocation();
      const response = await fetch(
        `https://courtcaller.azurewebsites.net/api/Branches/sortBranchByDistance?Latitude=${position.coords.latitude}&Longitude=${position.coords.longitude}&pageNumber=1&pageSize=${itemsPerPage}`
      );
      const data = await response.json();
      setBranches(data.data.map((item) => item.branch));
      setTotalBranches(data.total);
      await fetchPrices(data.data.branch);
      await fetchNumberOfCourts(data.data.branch);
    } catch (err) {
      setError("Failed to fetch data");
    } finally {
      setLoading(false);
    }
  };

  const handlePageChange = (page) => {
    setPageNumber(page);
    window.scrollTo(0, 0);
  };

  const handleBookNow = (branch) => {
    setSelectedBranch(branch);
    setModalIsOpen(true);
  };

  const closeModal = () => {
    setModalIsOpen(false);
  };

  const handleScheduleByDay = () => {
    navigate("/bookbyday", { state: { branch: selectedBranch } });
    scroll.scrollToTop({
      duration: 1000,
      smooth: "easeInOutQuart",
    });
  };

  const handleFixBooking = () => {
    navigate("/fixschedule", { state: { branch: selectedBranch } });
    scroll.scrollToTop({
      duration: 1000,
      smooth: "easeInOutQuart",
    });
  };

  const handleFlexBooking = () => {
    navigate("/flexible", { state: { branch: selectedBranch } });
    scroll.scrollToTop({
      duration: 1000,
      smooth: "easeInOutQuart",
    });
  };

  return (
    <>
      <div className="bg-gray-100">
        {/* Hero Banner */}
        <div
          className="h-[600px] w-full bg-cover bg-center bg-no-repeat flex items-center mb-5"
          style={{
            backgroundImage: `url('https://images.unsplash.com/photo-1551698618-1dfe5d97d256?w=1200&h=600&fit=crop')`,
          }}
        >
          <div className="pl-20">
            <span className="text-sm uppercase font-bold tracking-[4px] text-blue-600">
              TENNIS COURT
            </span>
            <h2 className="text-5xl uppercase font-bold tracking-[4px] leading-[52px] text-gray-900 my-3">
              BOOK YOUR COURT
            </h2>
          </div>
        </div>

        {/* Slideshow */}
        <div className="container mx-auto px-4">
          <div className="pt-2.5 pb-8">
            <SlideShowHomePage />
          </div>
        </div>

        {/* Search Section */}
        <div className="container mx-auto px-4">
          <div className="flex items-center justify-between mb-8">
            <div className="flex items-center">
              <select
                value={city}
                onChange={(e) => {
                  setCity(e.target.value);
                  setDistrict("");
                }}
                className="p-4 mx-2 border border-gray-300 rounded-md w-32 bg-white text-gray-700 focus:border-blue-600 focus:outline-none appearance-none"
              >
                <option value="">City</option>
                <option value="Hồ Chí Minh">Hồ Chí Minh</option>
                <option value="Hà Nội">Hà Nội</option>
                <option value="Đà Nẵng">Đà Nẵng</option>
              </select>

              <select
                value={district}
                onChange={(e) => setDistrict(e.target.value)}
                className="p-4 mx-2 border border-gray-300 rounded-md w-32 bg-white text-gray-700 focus:border-blue-600 focus:outline-none appearance-none"
              >
                <option value="">District</option>
                {city &&
                  cityDistricts[city].map((dist, index) => (
                    <option key={index} value={dist}>
                      {dist}
                    </option>
                  ))}
              </select>

              <button
                onClick={handleSearch}
                className="h-11 px-4 py-4 border-none bg-blue-600 text-white rounded-md cursor-pointer hover:bg-blue-700 transition-colors flex items-center justify-center"
              >
                <AiOutlineSearch className="text-xl" />
              </button>
            </div>

            <div className="ml-48">
              <button
                onClick={handleSortByDistance}
                className="h-11 px-4 py-4 border-none bg-blue-600 text-white rounded-md cursor-pointer hover:bg-blue-700 transition-colors"
              >
                Sort By Distance
              </button>
            </div>
          </div>
        </div>

        {/* Booking Branch Section */}
        <div className="container mx-auto px-4">
          <h1 className="flex justify-center mb-8 text-3xl font-bold">
            Booking Now
          </h1>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5 mb-8">
            {branches.map((branch, index) => (
              <div
                className="bg-white p-5 border border-gray-300 rounded-2xl text-center transition-all duration-300 hover:transform hover:-translate-y-2 hover:shadow-lg"
                key={index}
              >
                {branch.branchPicture && (
                  <img
                    className="mb-2 max-w-full h-auto block mx-auto h-58 w-full object-cover rounded-lg"
                    src={
                      isJson(branch.branchPicture)
                        ? JSON.parse(branch.branchPicture)[0]
                        : branch.branchPicture
                    }
                    alt="branch"
                  />
                )}
                <h3 className="mb-2 text-xl font-semibold">
                  {branch.branchName}
                </h3>
                <p className="mb-4 text-base text-gray-600">
                  Number of courts:{" "}
                  {numberOfCourts[branch.branchId] || "No court"}
                </p>
                <p className="mb-4 text-base text-gray-600">
                  Address: {branch.branchAddress}
                </p>
                <p className="mb-4 text-base text-gray-600">
                  {prices[branch.branchId]
                    ? `Weekday: ${prices[branch.branchId].weekdayPrice} VND`
                    : "Loading..."}
                </p>
                <p className="mb-4 text-base text-gray-600">
                  {prices[branch.branchId]
                    ? `Weekend: ${prices[branch.branchId].weekendPrice} VND`
                    : "Loading..."}
                </p>
                <button
                  onClick={() => handleBookNow(branch)}
                  className="px-5 py-2 bg-blue-600 text-white border-none rounded-md cursor-pointer hover:bg-blue-700 transition-colors"
                >
                  Book now
                </button>
              </div>
            ))}
          </div>

          {/* Pagination */}
          <div className="flex justify-center mt-8 gap-2">
            {Array.from({ length: totalPages }, (_, index) => (
              <button
                key={index}
                onClick={() => handlePageChange(index + 1)}
                className={`px-4 py-2 border border-gray-300 rounded-md cursor-pointer transition-all duration-300 ${
                  pageNumber === index + 1
                    ? "bg-blue-600 text-white border-blue-600"
                    : "bg-white text-gray-700 hover:bg-blue-600 hover:text-white"
                }`}
              >
                {index + 1}
              </button>
            ))}
          </div>
        </div>

        {/* Introduction Section */}
        <div className="container mx-auto px-4 py-8">
          <h1 className="text-center text-3xl font-bold mb-8">
            Lý do chọn đặt sân
          </h1>
          <div className="flex justify-center">
            <img
              src="https://images.unsplash.com/photo-1554068865-24cecd4e34b8?w=800&h=400&fit=crop"
              alt="hero"
              className="max-w-full h-auto rounded-lg shadow-lg"
            />
          </div>
        </div>

        {/* Features Section */}
        <div className="container mx-auto px-4 py-8">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
            <div className="text-center mb-5">
              <img
                src="https://images.unsplash.com/photo-1551698618-1dfe5d97d256?w=200&h=150&fit=crop"
                alt="feature"
                className="w-full max-w-32 mb-2 mx-auto rounded-lg"
              />
              <h3 className="mt-2 text-xl font-semibold">
                Đặt lịch nhanh chóng
              </h3>
              <p className="mt-2 text-base text-gray-600">
                Hoạt động trong 60 thành phố
              </p>
            </div>
            <div className="text-center mb-5">
              <img
                src="https://images.unsplash.com/photo-1622279457486-62dcc4a431d6?w=200&h=150&fit=crop"
                alt="feature"
                className="w-full max-w-32 mb-2 mx-auto rounded-lg"
              />
              <h3 className="mt-2 text-xl font-semibold">
                Tiết kiệm thời gian
              </h3>
              <p className="mt-2 text-base text-gray-600">
                Luôn đảm bảo đúng giờ
              </p>
            </div>
            <div className="text-center mb-5">
              <img
                src="https://images.unsplash.com/photo-1593766827796-c6d2f2d29f91?w=200&h=150&fit=crop"
                alt="feature"
                className="w-full max-w-32 mb-2 mx-auto rounded-lg"
              />
              <h3 className="mt-2 text-xl font-semibold">Chất lượng cao</h3>
              <p className="mt-2 text-base text-gray-600">
                Cung cấp chất lượng sân tốt nhất
              </p>
            </div>
            <div className="text-center mb-5">
              <img
                src="https://images.unsplash.com/photo-1551698618-1dfe5d97d256?w=200&h=150&fit=crop"
                alt="feature"
                className="w-full max-w-32 mb-2 mx-auto rounded-lg"
              />
              <h3 className="mt-2 text-xl font-semibold">Hỗ trợ 24/7</h3>
              <p className="mt-2 text-base text-gray-600">
                Luôn sẵn sàng hỗ trợ khách hàng
              </p>
            </div>
          </div>
        </div>

        {/* Modal */}
        <Modal
          isOpen={modalIsOpen}
          onRequestClose={closeModal}
          className="bg-white rounded-lg p-5 w-4/5 max-w-lg shadow-xl relative z-50"
          overlayClassName="fixed inset-0 bg-black bg-opacity-75 flex justify-center items-center z-40"
        >
          <h2 className="mb-5 text-xl text-center font-semibold">
            Select the Type of Booking
          </h2>
          <div className="flex justify-around">
            <button
              onClick={handleFixBooking}
              className="block w-1/4 py-2 mb-2 border-none rounded-md bg-blue-600 text-white cursor-pointer text-base transition-colors hover:bg-blue-700"
            >
              Fixed Schedule
            </button>
            <button
              onClick={handleScheduleByDay}
              className="block w-1/4 py-2 mb-2 border-none rounded-md bg-blue-600 text-white cursor-pointer text-base transition-colors hover:bg-blue-700"
            >
              Schedule by Day
            </button>
            <button
              onClick={handleFlexBooking}
              className="block w-1/4 py-2 mb-2 border-none rounded-md bg-blue-600 text-white cursor-pointer text-base transition-colors hover:bg-blue-700"
            >
              Flex Schedule
            </button>
          </div>
        </Modal>
      </div>
    </>
  );
};

export default memo(HomePage);
