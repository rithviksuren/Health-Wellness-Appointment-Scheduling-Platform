import { expect, test } from "@playwright/test";

test("donor can reach donation flow", async ({ page }) => {
  await page.goto("/");
  await page.getByRole("link", { name: "Start a Donation" }).click();
  await expect(page.getByRole("heading", { name: "Donate" })).toBeVisible();
});

