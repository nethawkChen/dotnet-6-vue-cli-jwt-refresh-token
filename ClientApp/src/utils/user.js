import req from "./axiosApi.js";
import { ref } from "vue";

export const userLogin = loginData => {
  console.log("loginData:", loginData);
  return req("post", "/auth/login", loginData);
};

export const userLogOut = () => {
  return req("post", "/token/revoke");
};

export const refreshLogin = () => {
  console.log("refreshLogin");
  const accessToken = sessionStorage.getItem("accessToken");
  const refreshToken = sessionStorage.getItem("refreshToken");
  const tokenApiModel = ref({
    accessToken: accessToken,
    refreshToken: refreshToken
  });
  console.log("refreshLogin:", tokenApiModel.value);
  return req("post", "/token/refresh", tokenApiModel.value);
};
