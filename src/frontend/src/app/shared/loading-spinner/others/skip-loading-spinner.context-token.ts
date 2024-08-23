import { HttpContextToken } from "@angular/common/http";

export const SkipLoadingSpinner = new HttpContextToken(() => false);
