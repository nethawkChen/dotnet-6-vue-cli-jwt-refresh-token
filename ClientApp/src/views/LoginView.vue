<template>
  <br />
  <br />
  <div class="container">
    <div class="row">
      <div class="col-3"></div>
      <div class="col-6">
        <h1>refresh token service</h1>
        <div class="card">
          <div class="card-header">使用者認證</div>
          <div class="card-body">
            <div class="input-group mb-3">
              <span class="input-group-text">帳號</span>
              <input type="text" class="form-control" v-model="loginData.userName" placeholder="NT Account" aria-label="NT Account" />
            </div>

            <div class="input-group mb-3">
              <span class="input-group-text">密碼</span>
              <input type="Password" class="form-control" v-model="loginData.password" placeholder="Password" aria-label="Password" />
            </div>

            <div class="input-group mb-3">
              <input type="submit" value="登入" v-on:click="uLogin" class="btn btn-primary offset-md-5" />
            </div>
          </div>
        </div>
      </div>
      <div class="col-3"></div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import { userLogin } from "@/utils/user.js";
import { useRouter } from "vue-router";

export default {
  setup() {
    const router = useRouter(); // 使用 useRouter 獲取路由的對象

    // 輸入登入資料
    const loginData = ref({
      userName: "testuser",
      password: "123@456",
      clientip: "123.321.xx.xx"
    });

    function uLogin() {
      userLogin(loginData.value)
        .then(res => {
          console.log("登入成功");
          //將accessToken儲存在sessionToken, 因為accessToken帶有 claim﹐做後續refresh_token時要取出user的用途
          sessionStorage.setItem("accessToken", res.data.accessToken);
          sessionStorage.setItem("refreshToken", res.data.refreshToken);
          sessionStorage.setItem("userName", loginData.value.userName);

          const redirectPath = sessionStorage.getItem("redirectPath");
          if (redirectPath) {
            router.push(redirectPath);
            sessionStorage.removeItem("redirectPath"); //自動導航後清除 redirectPath
          } else {
            router.push({ name: "Home" });
          }
        })
        .catch(error => {
          console.log("登入失敗");
          console.log("error:", error);
        });
    }

    return {
      loginData,
      uLogin
    };
  }
};
</script>

<style></style>
