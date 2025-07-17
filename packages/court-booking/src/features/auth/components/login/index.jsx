import React, { useState, useEffect } from 'react';
// import "./loginTest.scss";
import { FaGoogle, FaFacebookF } from 'react-icons/fa';
// import { loginApi } from "../../../user/api/usersApi";
import {
  validateFullName,
  validateEmail,
  validatePassword,
  validateConfirmPassword,
} from '../../../../components/shared/Validations/formValidation';
// import axios from "axios";
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { GoogleLogin } from '@react-oauth/google';
import ClipLoader from 'react-spinners/ClipLoader';
// import { ROUTERS } from "utils/router";
import { FacebookAuthProvider, signInWithPopup } from 'firebase/auth';
import { auth } from '../../../../firebase.js';
// import { useAuth } from "../../AuthContext.jsx";
import { jwtDecode } from 'jwt-decode';
import { useGetQuery } from '../../../../api/hook.js';
// import { fetchUserById, fetchRoleByUserId } from "api/userApi";

const Login = () => {
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [isLogin, setIsLogin] = useState(true);
  const [message, setMessage] = useState('');
  const [messageType, setMessageType] = useState('');
  const [loading, setLoading] = useState(false);
  const [profile, setProfile] = useState(null);
  const booking = useGetQuery('/api/Bookings/current-time', {});
  console.log('Current Booking Time:', booking.data);

  const [fullNameValidation, setFullNameValidation] = useState({
    isValid: true,
    message: '',
  });
  const [emailValidation, setEmailValidation] = useState({
    isValid: true,
    message: '',
  });
  const [passwordValidation, setPasswordValidation] = useState({
    isValid: true,
    message: '',
  });
  const [confirmPasswordValidation, setConfirmPasswordValidation] = useState({
    isValid: true,
    message: '',
  });

  // const { login } = useAuth();

  useEffect(() => {
    const container = document.getElementById('container');
    const registerBtn = document.getElementById('register');
    const loginBtn = document.getElementById('login');

    const addActiveClass = () => container.classList.add('active');
    const removeActiveClass = () => container.classList.remove('active');

    if (registerBtn && loginBtn) {
      registerBtn.addEventListener('click', addActiveClass);
      loginBtn.addEventListener('click', removeActiveClass);
    }

    return () => {
      if (registerBtn && loginBtn) {
        registerBtn.removeEventListener('click', addActiveClass);
        loginBtn.removeEventListener('click', removeActiveClass);
      }
    };
  }, []);

  // const handleLogin = async (e) => {
  //   e.preventDefault();

  //   if (!email || !password) {
  //     toast.error("Email/Password is required!");
  //     return;
  //   }

  //   setLoading(true);
  //   try {
  //     const res = await loginApi(email, password);
  //     if (res && res.token) {
  //       localStorage.setItem("token", res.token);
  //       console.log("token: ", res.token);
  //       var decode = jwtDecode(res.token);
  //       localStorage.setItem("userRole", decode.role);
  //       const userData = {
  //         email: decode.email,
  //         role: decode.role,
  //       };
  //       login(userData); // Lưu thông tin người dùng vào context
  //       // navigate(ROUTERS.USER.HOME);
  //     } else if (res && res.status === 401) {
  //       setMessage("Login failed!");
  //       setMessageType("error");
  //     } else if (
  //       res &&
  //       res.data.status === "Error" &&
  //       res.data.message == "User is banned!"
  //     ) {
  //       toast.error("This account is banned!", {
  //         position: "top-right",
  //         autoClose: 5000,
  //         hideProgressBar: false,
  //         closeOnClick: true,
  //         pauseOnHover: true,
  //         draggable: true,
  //         progress: undefined,
  //         theme: "colored",
  //       });
  //       return;
  //     }
  //   } catch (error) {
  //     setMessage("Login failed!");
  //     setMessageType("error");
  //   } finally {
  //     setLoading(false);
  //   }
  // };

  // const handleRegister = async (e) => {
  //   e.preventDefault();

  //   const fullNameValidation = validateFullName(fullName);
  //   const emailValidation = await validateEmail(email);
  //   const passwordValidation = validatePassword(password);
  //   const confirmPasswordValidation = validateConfirmPassword(
  //     password,
  //     confirmPassword
  //   );

  //   setFullNameValidation(fullNameValidation);
  //   setEmailValidation(emailValidation);
  //   setPasswordValidation(passwordValidation);
  //   setConfirmPasswordValidation(confirmPasswordValidation);

  //   if (
  //     !fullNameValidation.isValid ||
  //     !emailValidation.isValid ||
  //     !passwordValidation.isValid ||
  //     !confirmPasswordValidation.isValid
  //   ) {
  //     setMessage("Please try again");
  //     setMessageType("error");
  //     return;
  //   }

  //   setLoading(true);

  //   try {
  //     const response = await axios.post(
  //       "https://courtcaller.azurewebsites.net/api/authentication/register",
  //       {
  //         fullName: fullName,
  //         email: email,
  //         password: password,
  //         confirmPassword: confirmPassword,
  //       }
  //     );
  //     toast.success("Registration successful!");
  //     setMessage("SIGN UP SUCCESSFULLY - LOG IN NOW");
  //     setMessageType("success");
  //     setIsLogin(true);
  //   } catch (error) {
  //     if (error.response) {
  //       toast.error(error.response.data.message || "Registration failed");
  //     } else if (error.request) {
  //       toast.error("No response from server");
  //     } else {
  //       toast.error(error.message);
  //     }
  //   } finally {
  //     setLoading(false);
  //   }
  // };

  const loginGoogle = async (response) => {
    var token = response.credential;
    console.log('Google Token:', token);

    try {
      const res = await fetch(
        'https://courtcaller.azurewebsites.net/api/authentication/google-login?token=' +
          token,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
        }
      );

      const data = await res.json();

      if (res.ok) {
        localStorage.setItem('token', data.token);
        localStorage.setItem('ggToken', token);
        var decode = jwtDecode(data.token);
        localStorage.setItem('userRole', decode.role);
        const userData = {
          email: decode.email,
          role: decode.role,
        };
        login(userData);
        // navigate(ROUTERS.USER.HOME);
      } else {
        console.error('Backend error:', data);
        toast.error('Login Failed');
        throw new Error(data.message || 'Google login failed');
      }
    } catch (error) {
      console.error('Error during login:', error);
      toast.error('Login Failed');
    }
  };

  const loginFacebook = async (response) => {
    try {
      const provider = new FacebookAuthProvider();
      const result = await signInWithPopup(auth, provider);

      const accessToken = result.user.stsTokenManager.accessToken;

      console.log('Login successfully', result.user);
      console.log('Access Token:', accessToken);

      const res = await fetch(
        'https://courtcaller.azurewebsites.net/api/authentication/facebook-login?token=' +
          accessToken,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
        }
      );

      const data = await res.json();

      if (res.ok) {
        console.log('Login successful:', data);
        localStorage.setItem('token', accessToken);
        var decode = jwtDecode(accessToken);

        const userData = {
          email: decode.email,
        };
        login(userData);
        // navigate(ROUTERS.USER.HOME);
      } else {
        console.error('Backend error:', data);
        toast.error('Login Failed');
      }
    } catch (error) {
      console.error('Error:', error.message);
      toast.error('Facebook login failed');
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-r from-gray-100 to-green-200 flex items-center justify-center p-4 font-['Montserrat']">
      <div
        className={`relative bg-white rounded-3xl shadow-lg overflow-hidden w-full max-w-4xl h-[520px] transition-all duration-600 ${!isLogin ? 'active' : ''}`}
      >
        {/* Sign In Form */}
        <div
          className={`absolute top-0 left-0 w-1/2 h-full transition-all duration-600 ease-in-out z-10 ${!isLogin ? 'transform translate-x-full' : ''}`}
        >
          {isLogin && (
            <div className='bg-white flex items-center justify-center flex-col px-10 h-full'>
              <div className='flex flex-col items-center w-full'>
                <h1 className='text-2xl font-bold mb-6 text-black'>LOG IN</h1>

                <div className='mb-5 w-full'>
                  <GoogleLogin
                    onSuccess={loginGoogle}
                    onError={() => {
                      console.log('Login Failed');
                    }}
                  />
                  <button
                    type='button'
                    onClick={loginFacebook}
                    className='mt-4 flex items-center justify-center bg-blue-600 text-white rounded-md px-4 py-2 w-full hover:bg-blue-700 transition-colors'
                  >
                    <FaFacebookF className='mr-2' />
                    Continue with Facebook
                  </button>
                </div>

                <span className='text-sm text-gray-600 mb-4'>
                  or use your account for login
                </span>

                <input
                  type='text'
                  value={email}
                  placeholder='Email'
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className={`w-full bg-gray-100 text-black border-none rounded-lg px-4 py-3 text-sm mb-2 outline-none ${
                    !emailValidation.isValid ? 'shadow-red-500 shadow-md' : ''
                  }`}
                />
                {emailValidation.message && (
                  <p className='text-red-500 text-xs font-semibold mb-2'>
                    {emailValidation.message}
                  </p>
                )}

                <input
                  type='password'
                  value={password}
                  placeholder='Password'
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className={`w-full bg-gray-100 text-black border-none rounded-lg px-4 py-3 text-sm mb-2 outline-none ${
                    !passwordValidation.isValid
                      ? 'shadow-red-500 shadow-md'
                      : ''
                  }`}
                />
                {passwordValidation.message && (
                  <p className='text-red-500 text-xs font-semibold mb-2'>
                    {passwordValidation.message}
                  </p>
                )}

                <div className='mb-4'>
                  <a to='/forget-password'>Forgot Password?</a>
                </div>

                <button
                  // onClick={handleLogin}
                  disabled={loading}
                  className='bg-green-600 text-white text-xs font-semibold uppercase tracking-wider py-3 px-11 rounded-lg border border-transparent hover:bg-green-700 transition-colors disabled:opacity-50'
                >
                  {loading ? <ClipLoader size={15} color='#fff' /> : 'Sign In'}
                </button>

                {message && (
                  <p
                    className={`mt-4 text-center ${messageType === 'error' ? 'text-red-500' : 'text-green-500'}`}
                  >
                    {message}
                  </p>
                )}
              </div>
            </div>
          )}
        </div>

        {/* Sign Up Form */}
        <div
          className={`absolute top-0 left-0 w-1/2 h-full transition-all duration-600 ease-in-out z-0 ${
            !isLogin
              ? 'transform translate-x-full opacity-100 z-20 animate-pulse'
              : 'opacity-0 z-10'
          }`}
        >
          {!isLogin && (
            <div className='bg-white flex items-center justify-center flex-col px-10 h-full'>
              <div className='flex flex-col items-center w-full'>
                <h1 className='text-2xl font-bold mb-6 text-black'>
                  Create Account
                </h1>

                <span className='text-sm text-gray-600 mb-4'>
                  or use your email for registration
                </span>

                <input
                  type='text'
                  value={fullName}
                  placeholder='Full Name'
                  onChange={(e) => setFullName(e.target.value)}
                  required
                  className={`w-full bg-gray-100 text-black border-none rounded-lg px-4 py-3 text-sm mb-2 outline-none ${
                    !fullNameValidation.isValid
                      ? 'shadow-red-500 shadow-md'
                      : ''
                  }`}
                />
                {fullNameValidation.message && (
                  <p className='text-red-500 text-xs font-semibold mb-2'>
                    {fullNameValidation.message}
                  </p>
                )}

                <input
                  type='text'
                  value={email}
                  placeholder='Email'
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className={`w-full bg-gray-100 text-black border-none rounded-lg px-4 py-3 text-sm mb-2 outline-none ${
                    !emailValidation.isValid ? 'shadow-red-500 shadow-md' : ''
                  }`}
                />
                {emailValidation.message && (
                  <p className='text-red-500 text-xs font-semibold mb-2'>
                    {emailValidation.message}
                  </p>
                )}

                <input
                  type='password'
                  value={password}
                  placeholder='Password'
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className={`w-full bg-gray-100 text-black border-none rounded-lg px-4 py-3 text-sm mb-2 outline-none ${
                    !passwordValidation.isValid
                      ? 'shadow-red-500 shadow-md'
                      : ''
                  }`}
                />
                {passwordValidation.message && (
                  <p className='text-red-500 text-xs font-semibold mb-2'>
                    {passwordValidation.message}
                  </p>
                )}

                <input
                  type='password'
                  value={confirmPassword}
                  placeholder='Confirm Password'
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                  className={`w-full bg-gray-100 text-black border-none rounded-lg px-4 py-3 text-sm mb-2 outline-none ${
                    !confirmPasswordValidation.isValid
                      ? 'shadow-red-500 shadow-md'
                      : ''
                  }`}
                />
                {confirmPasswordValidation.message && (
                  <p className='text-red-500 text-xs font-semibold mb-4'>
                    {confirmPasswordValidation.message}
                  </p>
                )}

                <button
                  // onClick={handleRegister}
                  disabled={loading}
                  className='bg-blue-500 text-white text-xs font-semibold uppercase tracking-wider py-3 px-11 rounded-lg border border-transparent hover:bg-blue-600 transition-colors disabled:opacity-50'
                >
                  {loading ? <ClipLoader size={15} color='#fff' /> : 'Sign Up'}
                </button>

                {message && (
                  <p
                    className={`mt-4 text-center ${messageType === 'error' ? 'text-red-500' : 'text-green-500'}`}
                  >
                    {message}
                  </p>
                )}
              </div>
            </div>
          )}
        </div>

        {/* Toggle Container */}
        <div
          className={`absolute top-0 left-1/2 w-1/2 h-full overflow-hidden transition-all duration-600 ease-in-out z-50 ${
            !isLogin
              ? 'transform -translate-x-full rounded-r-full rounded-tl-none rounded-bl-none'
              : 'rounded-l-full rounded-tr-none rounded-br-none'
          }`}
        >
          <div
            className={`relative bg-gradient-to-r from-blue-400 to-green-500 h-full text-white w-[200%] transition-all duration-600 ease-in-out ${
              !isLogin ? 'transform translate-x-1/2' : 'transform translate-x-0'
            } -left-full`}
          >
            {/* Toggle Left Panel */}
            <div
              className={`absolute w-1/2 h-full flex items-center justify-center flex-col px-8 text-center transition-all duration-600 ease-in-out ${
                !isLogin
                  ? 'transform translate-x-0'
                  : 'transform -translate-x-full'
              }`}
            >
              <h1 className='text-xl font-bold uppercase mb-4'>
                badminton is joy
              </h1>
              <p className='text-sm leading-5 tracking-wide mb-5 uppercase'>
                Enter your username & password to schedule now!!
              </p>
              <button
                onClick={() => setIsLogin(true)}
                className='bg-transparent border border-white text-white text-xs font-semibold uppercase tracking-wider py-3 px-11 rounded-lg hover:bg-white hover:text-green-500 transition-colors'
              >
                Sign In
              </button>
            </div>

            {/* Toggle Right Panel */}
            <div
              className={`absolute right-0 w-1/2 h-full flex items-center justify-center flex-col px-8 text-center transition-all duration-600 ease-in-out ${
                !isLogin
                  ? 'transform translate-x-full'
                  : 'transform translate-x-0'
              }`}
            >
              <h1 className='text-xl font-bold uppercase mb-4'>
                badminton is life
              </h1>
              <p className='text-sm leading-5 tracking-wide mb-5 uppercase'>
                Register with your personal details to use all of the site
                features!!
              </p>
              <button
                onClick={() => setIsLogin(false)}
                className='bg-transparent border border-white text-white text-xs font-semibold uppercase tracking-wider py-3 px-11 rounded-lg hover:bg-white hover:text-blue-500 transition-colors'
              >
                Sign Up
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
