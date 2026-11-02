import { expect, test } from "@playwright/test";
import { seedAuthedSession, TEST_USER } from "../helpers/auth-seed";
import { installAdminShellMocks, ADMIN_PERMS, paged } from "../helpers/shell-mocks";
const base = "**/api/v1/catalog/countries";
const permissions = ["View","Create","Update","Delete"].map(action => "Permissions.Catalog.Countries."+action);
const country = { id:"d6f6541a-3ac0-41eb-b86a-536c2b4c7281", name:"Brasil",iso2Code:"BR",iso3Code:"BRA",numericCode:"076",phoneCode:"+55",isActive:true };

test.beforeEach(async ({page}) => {
  const perms = [...ADMIN_PERMS,...permissions];
  await seedAuthedSession(page,{...TEST_USER,permissions:perms});
  await installAdminShellMocks(page,perms);
  await page.route(base+"?*",route=>route.fulfill({json:paged([country])}));
});

test("lists Portuguese columns and searches, filters and sorts through the API",async({page})=>{
  await page.goto("/countries");
  await expect(page.getByRole("heading",{name:"Países",exact:true})).toBeVisible();
  await expect(page.getByRole("cell",{name:"076",exact:true})).toBeVisible();
  const search = page.waitForRequest(request=>request.url().includes("search=BR"));
  await page.getByRole("textbox",{name:"Pesquisar países"}).fill("BR");
  await search;
  await page.getByRole("button",{name:"Todas",exact:true}).click();
  const inactive = page.waitForRequest(request=>request.url().includes("isActive=false"));
  await page.getByRole("menuitem",{name:"Inativos",exact:true}).click();
  await inactive;
  await page.getByRole("button",{name:"Crescente",exact:true}).click();
  const sorting = page.waitForRequest(request=>request.url().includes("sortDir=desc"));
  await page.getByRole("menuitem",{name:"Decrescente",exact:true}).click();
  await sorting;
});

test("creates a country with normalized ISO codes and leading zeroes",async({page})=>{
  await page.route(base,async route=>{
    if(route.request().method()==="POST") await route.fulfill({json:country.id});
    else await route.fallback();
  });
  await page.goto("/countries");
  await page.getByRole("button",{name:"Novo país",exact:true}).click();
  const dialog=page.getByRole("dialog");
  await dialog.getByRole("textbox",{name:"País",exact:true}).fill("  Brasil  ");
  await dialog.getByRole("textbox",{name:"Código ISO 2",exact:true}).fill("br");
  await dialog.getByRole("textbox",{name:"Código ISO 3",exact:true}).fill("bra");
  await dialog.getByRole("textbox",{name:"Código Numérico ISO",exact:true}).fill("076");
  await dialog.getByRole("textbox",{name:"Código Telefônico",exact:true}).fill("+55");
  const posted=page.waitForRequest(request=>request.method()==="POST" && request.url().endsWith("/countries"));
  await dialog.getByRole("button",{name:"Salvar",exact:true}).click();
  expect((await posted).postDataJSON()).toMatchObject({name:"Brasil",iso2Code:"BR",iso3Code:"BRA",numericCode:"076",isActive:true});
  await expect(dialog).not.toBeVisible();
});

test("shows required and format errors beside fields",async({page})=>{
  await page.goto("/countries");
  await page.getByRole("button",{name:"Novo país",exact:true}).click();
  await page.getByRole("dialog").getByRole("button",{name:"Salvar",exact:true}).click();
  await expect(page.getByText("Informe o país com até 150 caracteres.",{exact:true})).toBeVisible();
  await expect(page.getByText("Informe exatamente 2 letras.",{exact:true})).toBeVisible();
});

test("edits and confirms activation and deletion",async({page})=>{
  await page.route(base+"/*",route=>route.fulfill({json:country.id}));
  await page.goto("/countries");
  await page.getByRole("button",{name:"Editar Brasil",exact:true}).click();
  await page.getByRole("textbox",{name:"País",exact:true}).fill("Brasil atualizado");
  const updated = page.waitForRequest(r=>r.method()==="PUT");
  await page.getByRole("dialog").getByRole("button",{name:"Salvar",exact:true}).click();
  expect((await updated).postDataJSON()).toMatchObject({name:"Brasil atualizado"});
  await page.getByRole("button",{name:"Desativar",exact:true}).click();
  const disabled=page.waitForRequest(r=>r.method()==="PUT");
  await page.getByRole("dialog").getByRole("button",{name:"Confirmar",exact:true}).click();
  expect((await disabled).postDataJSON()).toMatchObject({isActive:false});
  await expect(page.getByRole("dialog")).not.toBeVisible();
  await page.getByRole("button",{name:"Excluir Brasil",exact:true}).click();
  const deleted=page.waitForRequest(r=>r.method()==="DELETE");
  await page.getByRole("dialog").getByRole("button",{name:"Excluir",exact:true}).click();
  await deleted;
  await expect(page.getByRole("dialog")).not.toBeVisible();
});

test("shows API duplicate errors against the correct field",async({page})=>{
  await page.route(base,route=>route.fulfill({status:400,json:{status:400,title:"Validação",errors:{Iso2Code:["Código ISO 2 já cadastrado."]}}}));
  await page.goto("/countries");
  await page.getByRole("button",{name:"Novo país",exact:true}).click();
  await page.getByRole("textbox",{name:"País",exact:true}).fill("Brasil");
  await page.getByRole("textbox",{name:"Código ISO 2",exact:true}).fill("BR");
  await page.getByRole("textbox",{name:"Código ISO 3",exact:true}).fill("BRA");
  await page.getByRole("dialog").getByRole("button",{name:"Salvar",exact:true}).click();
  await expect(page.locator("#country-iso2Code-error")).toHaveText("Código ISO 2 já cadastrado.");
});

test("view-only permission hides mutation controls",async({page})=>{
  const perms=[...ADMIN_PERMS,permissions[0]];
  await seedAuthedSession(page,{...TEST_USER,permissions:perms});
  await installAdminShellMocks(page,perms);
  await page.goto("/countries");
  await expect(page.getByRole("cell",{name:"Brasil",exact:true})).toBeVisible();
  await expect(page.getByRole("button",{name:"Novo país",exact:true})).toHaveCount(0);
  await expect(page.getByRole("button",{name:"Editar Brasil",exact:true})).toHaveCount(0);
});

test("missing view permission denies route",async({page})=>{
  await seedAuthedSession(page,{...TEST_USER,permissions:[...ADMIN_PERMS]});
  await installAdminShellMocks(page,ADMIN_PERMS);
  await page.goto("/countries");
  await expect(page.getByRole("heading",{name:"Países",exact:true})).toHaveCount(0);
  await expect(page.getByText("Permissions.Catalog.Countries.View",{exact:true})).toBeVisible();
});
