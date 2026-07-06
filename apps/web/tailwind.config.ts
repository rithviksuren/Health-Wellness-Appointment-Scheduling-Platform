import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./app/**/*.{ts,tsx}", "./components/**/*.{ts,tsx}", "./lib/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        ink: "#18211f",
        leaf: "#2d6a4f",
        mint: "#d8f3dc",
        gold: "#c99700",
        coral: "#d95d39",
        cloud: "#f7f8f5"
      },
      boxShadow: {
        soft: "0 12px 30px rgba(24, 33, 31, 0.08)"
      }
    }
  },
  plugins: []
};

export default config;

