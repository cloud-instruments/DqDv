import { environmentDefaults } from "./environment.default";

export const environment = {
    ...environmentDefaults,
    production: false,
    serverBaseUrl: "http://localhost:12628/"
};
