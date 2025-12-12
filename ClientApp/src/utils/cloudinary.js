// src/utils/cloudinary.js

/**
 * 上傳圖片到 Cloudinary，使用後端簽章的方式
 * @param {File} file - 要上傳的圖片檔案
 * @param {string} folder - 指定儲存資料夾（例如 blog_featured/abc123）
 * @returns {Promise<string>} 圖片 secure_url
 */
export async function uploadImageToCloudinary(file, folder) {
    try {
        const folderParam = encodeURIComponent(folder);
        const res = await fetch(`/api/cloudinary/sign?folder=${folderParam}`);
        const signData = await res.json();

        const formData = new FormData();
        formData.append('file', file);
        formData.append('api_key', signData.apiKey);
        formData.append('timestamp', signData.timestamp);
        formData.append('signature', signData.signature);
        formData.append('folder', signData.folder);

        const uploadRes = await fetch(
            `https://api.cloudinary.com/v1_1/${signData.cloudName}/image/upload`,
            {
                method: 'POST',
                body: formData,
            }
        );

        const data = await uploadRes.json();

        if (!data.secure_url) {
            throw new Error(data.error?.message || 'Cloudinary 上傳失敗');
        }

        return data.secure_url;
    } catch (error) {
        console.error('圖片上傳失敗:', error);
        throw error;
    }
}