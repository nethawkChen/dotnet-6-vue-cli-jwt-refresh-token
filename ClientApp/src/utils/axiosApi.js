import axios from "axios";
import { refreshLogin } from "./user";

const instance = axios.create({
  baseURL: process.env.VUE_APP_API_LOCAL, // 由環境變數設定網址
  headers: { "Content-Type": "application/json" },
  timeout: 20000
});

//此處的instance為我們create的實體
//request攔截器
//  第一個函式會在request送出攔截到此次的config﹐可以做最後的處理
//  第二個函式可以在request發生錯誤時做一些額外的處理
instance.interceptors.request.use(
  function (config) {
    const token = sessionStorage.getItem("accessToken");
    config.headers.Authorization = "Bearer " + token; // 固定每次送出時都會加上這個 header
    console.log("request success:", token);
    return config;
  },
  function (error) {
    console.log("request error:", error);
    return Promise.reject(error);
  }
);

//此處的instance為我們create的實體
//response擱截器
instance.interceptors.response.use(
  function (response) {
    // Do something with response data
    console.log("axios response 攔截器 success.", response);
    return response;
  },
  function (error) {
    console.log("axios response 攔截器 error.", error);
    if (error.response.status === 401) {
      console.log("未授權, Token 已過期, 置換 token 開始");
      return refreshLogin()
        .then(res => {
          console.log("axios 401 error res:", res);
          if (res.status === 200) {
            sessionStorage.setItem("accessToken", res.data.accessToken);
            sessionStorage.setItem("refreshToken", res.data.refreshToken);

            // Retry the original request
            return retryOriginalRequest(error.config);
          }
        })
        .catch(rep => {
          console.log("axios 401 error catch rep:", rep);
          console.log("rep.response.data:", rep.response.data);
          if (rep.response.status === 400 || rep.response.data === "Invalid client request") {
            console.log("refresh token 需要重新登入");
          }
          return Promise.reject(error);
        });
    } else if (error.response.status === 404) {
      console.log("你要找的頁面不存在");
    } else if (error.response.status === 500) {
      console.log("程式發生問題");
    } else {
      console.log(error.message);
    }

    if (!window.navigator.onLine) {
      alert("網路出了點問題﹐請重新連線後重整網頁");
      return;
    }
    return Promise.reject(error);
  }
);

// 調用原本的 request
export function retryOriginalRequest(config) {
  console.log("retryOriginalRequest:", config);

  return new Promise((resolve, reject) => {
    const cancelToken = axios.CancelToken;
    const source = cancelToken.source();

    // set up a request to be canceled if it takes too long
    const timeout = setTimeout(() => {
      source.cancel();
      reject(new Error("retryOriginalRequest Request timed out"));
    }, 20000); // set your desired timeout value

    // make the original request
    instance({
      ...config,
      cancelToken: source.token
    })
      .then(response => {
        clearTimeout(timeout);
        resolve(response);
      })
      .catch(error => {
        clearTimeout(timeout);
        reject(error);
      });
  });
}

export default function (method, url, data = null, config) {
  method = method.toLowerCase();
  switch (method) {
    case "post":
      return instance.post(url, data, config);
    case "get":
      return instance.get(url, { params: data });
    case "delete":
      return instance.delete(url, { params: data });
    case "put":
      return instance.put(url, data);
    case "patch":
      return instance.patch(url, data);
    default:
      console.log(`未知的 method: ${method}`);
      return false;
  }
}
