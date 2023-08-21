<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-light">
    <div class="container-fluid">
      <a class="navbar-brand" href="#">refresh token</a>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarSupportedContent">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li class="nav-item">
            <a class="nav-link active" aria-current="page" href="#">Home</a>
          </li>
          <li class="nav-item">
            <router-link to="Test" class="nav-link">Test</router-link>
          </li>
        </ul>

        <router-link v-if="notLogin" to="/login" custom v-slot="{ navigate }">
          <button @click="navigate" role="link">登入</button>
        </router-link>
        <span v-if="!notLogin" style="font-weight: bold">登入者：{{ userName }}</span>
        <button v-if="!notLogin" class="btn btn-primary" @click="logoutUser">登出</button>
      </div>
    </div>
  </nav>
</template>

<script>
import { onMounted, ref } from "vue";
import { userLogOut } from "@/utils/user.js";
import { useRouter } from "vue-router";

export default {
  setup() {
    const router = useRouter();

    const userName = ref("");
    const notLogin = ref(true);
    const access_token = ref("");

    function logoutUser() {
      if (!confirm("是否確定要登出?")) {
        return;
      }

      userLogOut()
        .then(resp => {
          console.log("登出成功!", resp);
          sessionStorage.removeItem("accessToken");
          sessionStorage.removeItem("refreshToken");
          router.push({ name: "home" });
        })
        .catch(error => {
          console.log("登出失敗!", error);
          return;
        });
    }

    onMounted(() => {
      console.log("onMounted");
      userName.value = sessionStorage.getItem("userName");
      access_token.value = sessionStorage.getItem("accessToken");
      if (access_token.value == undefined) {
        notLogin.value = true;
      } else {
        notLogin.value = false;
      }
    });

    return {
      userName,
      notLogin,
      access_token,
      logoutUser
    };
  }
};
</script>

<style></style>
