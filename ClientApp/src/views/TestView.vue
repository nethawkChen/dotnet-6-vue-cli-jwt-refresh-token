<template>
  <router-view name="Header"></router-view>
  <div>
    這是 Test page -
    <span style="font-weight: bold">要驗證</span>
    <br />
    name：
    <input type="text" v-model="params.name" />
    msg：
    <input type="text" v-model="params.msg" />
    <br />
    <button id="btn" @click="sayHelloApi">Call Jwt 驗證的 API</button>
    <br />
    {{ resData }}
  </div>
</template>

<script>
import { SayHello } from "@/utils/testapi";
import { ref } from "vue";

export default {
  setup() {
    const params = ref({
      name: "Tom",
      msg: "Come on"
    });

    const resData = ref("");

    const sayHelloApi = () => {
      return new Promise((resolve, reject) => {
        SayHello(params.value)
          .then(result => {
            console.log("result:", result);
            resData.value = result.data;
            resolve(result);
          })
          .catch(error => {
            console.log("error:", error);
            resData.value = "error:" + error.data;
            reject(error);
          });
      });
    };

    return {
      params,
      resData,
      sayHelloApi
    };
  }
};
</script>

<style></style>
