<template>
  <div class="uploader">
    <label class="upload-box"
           @dragover.prevent
           @drop.prevent="onDrop">
      <div v-if="!previewUrl" class="placeholder">
        <p>點擊或拖曳圖片上傳</p>
        <input type="file" accept="image/*" @change="onFileChange" hidden ref="fileInput" />
        <button @click="triggerInput" type="button">選擇圖片</button>
      </div>

      <div v-if="previewUrl" class="preview">
        <img :src="previewUrl" alt="preview" />
        <button type="button" @click="reset">重新選擇</button>
      </div>

      <div v-if="loading" class="loading">
        上傳中...
      </div>
    </label>
  </div>
</template>

<script setup>
import { ref } from "vue";

// emit 回傳 Cloudinary 上傳後的 URL 給父層
const emit = defineEmits(["uploaded"]);

const previewUrl = ref(null);
const loading = ref(false);
const fileInput = ref(null);

// 你後端的 sign API
const SIGN_API = "https://localhost:52930/api/Cloudinary/sign"; 

// 點擊打開檔案選擇器
const triggerInput = () => fileInput.value.click();

// 處理檔案選擇
const onFileChange = async (e) => {
  const file = e.target.files[0];
  if (file) await uploadToCloudinary(file);
};

// 拖曳上傳
const onDrop = async (e) => {
  const file = e.dataTransfer.files[0];
  if (file) await uploadToCloudinary(file);
};

// 重設元件
const reset = () => {
  previewUrl.value = null;
  emit("uploaded", null);
};

// 核心：上傳圖片到 Cloudinary
const uploadToCloudinary = async (file) => {
  loading.value = true;

  try {
    // 【STEP 1】向你的後端拿 signature
    const folder = "news"; // 你想存放的 Cloudinary 資料夾
    const signRes = await fetch(`${SIGN_API}?folder=${folder}`);
    const signData = await signRes.json();

    // 【STEP 2】準備 FormData 給 Cloudinary
    const formData = new FormData();
    formData.append("file", file);
    formData.append("api_key", signData.apiKey);
    formData.append("timestamp", signData.timestamp);
    formData.append("signature", signData.signature);
    formData.append("folder", folder);

    const cloudName = signData.cloudName;

    // 【STEP 3】上傳至 Cloudinary
    const uploadRes = await fetch(
      `https://api.cloudinary.com/v1_1/${cloudName}/image/upload`,
      { method: "POST", body: formData }
    );
    const uploadData = await uploadRes.json();

    // 【STEP 4】成功 → 產生預覽 & 回傳 URL 給父元件
    previewUrl.value = uploadData.secure_url;

    emit("uploaded", uploadData.secure_url);
  } catch (err) {
    console.error("Upload Failed:", err);
    alert("圖片上傳失敗！");
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.uploader {
  width: 100%;
}

.upload-box {
  border: 2px dashed #ccc;
  padding: 20px;
  text-align: center;
  border-radius: 10px;
  cursor: pointer;
  display: block;
  transition: background 0.2s;
}

.upload-box:hover {
  background: #f8f8f8;
}

.placeholder p {
  margin-bottom: 10px;
}

.preview img {
  max-width: 100%;
  border-radius: 8px;
}

.loading {
  margin-top: 10px;
  color: #888;
}
</style>
