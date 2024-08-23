const V1_BASE_URL = "http://localhost:5065/api/v1/";
export const environment = {
  API_BASE_URL: {
    V1: "http://localhost:5065/api/v1/"
  },
  V1: {
    BASE_URL: V1_BASE_URL,
    LOGIN: V1_BASE_URL.concat("account/login"),
    SIGN_UP: V1_BASE_URL.concat("account/signup"),
    CONFIRM_ACCOUNT: V1_BASE_URL.concat("account/confirm"),
    FORGOT_PASSWORD: V1_BASE_URL.concat("account/forgot-password"),
    RESET_PASSWORD: V1_BASE_URL.concat("account/reset-password"),
    RESEND_VERIFICATION_CODE: V1_BASE_URL.concat("account/resend-verification-code"),
    CHECK_TOKEN: V1_BASE_URL.concat("account/check-token"),
    LOAD_XSRF_TOKEN: V1_BASE_URL.concat("load-xsrf-token"),
    LOGOUT: V1_BASE_URL.concat("account/logout"),
    QUESTION: V1_BASE_URL.concat("question"),
    Tags: V1_BASE_URL.concat("tag/all")
  },
  RECAPTCHA_V3_SITE_KEY: "6Lc-JL0pAAAAAF8dJRJeFWDW4zynSaAZkd1B_F-v",
  PRODUCTION: false,
};
