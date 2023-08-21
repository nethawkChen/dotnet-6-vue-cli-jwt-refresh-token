import req from "./axiosApi.js";

export const SayHello = value => {
  console.log("SayHello value:", value);
  return req("post", "/test/SayHello", value);
};
