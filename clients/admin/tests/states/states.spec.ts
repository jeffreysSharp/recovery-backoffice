import { expect, test } from "@playwright/test";
import { seedAuthedSession, TEST_USER } from "../helpers/auth-seed";
import { installAdminShellMocks, ADMIN_PERMS, paged } from "../helpers/shell-mocks";
const base = "**/api/v1/catalog/states";
const permissions = ["View","Create","Update","Delete"].map(action => "Permissions.Catalog.States."+action);
const br = { id:"d6f6541a-3ac0-41eb-b86a-536c2b4c7281", name:"Brasil",iso2Code:"BR",iso3Code:"BRA",numericCode:"076",phoneCode:"+55",isActive:true };
const pt = { ...br, id:"d6f6541a-3ac0-41eb-b86a-536c2b4c7282", name:"Portugal", iso2Code:"PT", iso3Code:"PRT" };
const us = { ...br, id:"d6f6541a-3ac0-41eb-b86a-536c2b4c7283", name:"Estados Unidos", iso2Code:"US", iso3Code:"USA" };
const state = { id:"d6f6541a-3ac0-41eb-b86a-536c2b4c7291",countryId:br.id,countryName:br.name,name:"São Paulo",code:"SP",ibgeCode:"35",isActive:true };

test.beforeEach(async({page})=>{
  const perms=[...ADMIN_PERMS,...permissions,"Permissions.Catalog.Countries.View"];
  await seedAuthedSession(page,{...TEST_USER,permissions:perms});
  await installAdminShellMocks(page,perms);
  await page.route("**/api/v1/catalog/countries?*",route=>{
    const pageNumber=Number(new URL(route.request().url()).searchParams.get("pageNumber"));
    const items=pageNumber===1 ? [br,pt,...Array.from({length:18},(_,index)=>({...br,id:"extra-"+index,name:"País "+index}))] : [us];
    return route.fulfill({json:{...paged(items),pageNumber,totalPages:2,totalCount:21}});
  });
  await page.route(base+"?*",route=>route.fulfill({json:paged([state])}));
});

test("lists states and filters by country, status and search",async({page})=>{
  await page.goto("/states");
  await expect(page.getByRole("heading",{name:"Estados",exact:true})).toBeVisible();
  await expect(page.getByRole("cell",{name:"35",exact:true})).toBeVisible();
  await page.getByRole("button",{name:"Todos os países",exact:true}).click();
  const filtered=page.waitForRequest(r=>r.url().includes("countryId="+pt.id));
  await page.getByRole("menuitem",{name:"Portugal",exact:true}).click();
  await filtered;
  const searched=page.waitForRequest(r=>r.url().includes("search=SP"));
  await page.getByRole("textbox",{name:"Pesquisar estados"}).fill("SP");
  await searched;
  await page.getByRole("button",{name:"Todas",exact:true}).click();
  const inactive=page.waitForRequest(r=>r.url().includes("isActive=false"));
  await page.getByRole("menuitem",{name:"Inativos",exact:true}).click();
  await inactive;
});

test("creates Brazilian state with normalized code and IBGE",async({page})=>{
  await page.route(base,route=>route.fulfill({json:state.id}));
  await page.goto("/states");
  await page.getByRole("button",{name:"Novo estado",exact:true}).click();
  const dialog=page.getByRole("dialog");
  await dialog.getByRole("button",{name:"País",exact:true}).click();
  await page.getByRole("menuitem",{name:"Brasil",exact:true}).click();
  await dialog.getByRole("textbox",{name:"Estado",exact:true}).fill("  São Paulo ");
  await dialog.getByRole("textbox",{name:"Sigla",exact:true}).fill("sp");
  await dialog.getByRole("textbox",{name:"Código IBGE",exact:true}).fill("35");
  const posted=page.waitForRequest(r=>r.method()==="POST" && r.url().endsWith("/states"));
  await dialog.getByRole("button",{name:"Salvar",exact:true}).click();
  expect((await posted).postDataJSON()).toMatchObject({countryId:br.id,name:"São Paulo",code:"SP",ibgeCode:"35",isActive:true});
  await expect(dialog).not.toBeVisible();
});

test("loads countries beyond first page and permits international codes without IBGE",async({page})=>{
  await page.route(base,route=>route.fulfill({json:state.id}));
  await page.goto("/states");
  await page.getByRole("button",{name:"Novo estado",exact:true}).click();
  const dialog=page.getByRole("dialog");
  await dialog.getByRole("button",{name:"País",exact:true}).click();
  await page.getByRole("menuitem",{name:"Estados Unidos",exact:true}).click();
  await expect(dialog.getByRole("textbox",{name:"Código IBGE",exact:true})).toBeDisabled();
  await dialog.getByRole("textbox",{name:"Estado",exact:true}).fill("California");
  await dialog.getByRole("textbox",{name:"Sigla",exact:true}).fill("abc-123");
  const posted=page.waitForRequest(r=>r.method()==="POST");
  await dialog.getByRole("button",{name:"Salvar",exact:true}).click();
  expect((await posted).postDataJSON()).toMatchObject({countryId:us.id,code:"ABC-123",ibgeCode:null});
});

test("changing Brazil to another country clears IBGE before saving",async({page})=>{
  await page.route(base+"/*",route=>route.fulfill({json:state.id}));
  await page.goto("/states");
  await page.getByRole("button",{name:"Editar São Paulo",exact:true}).click();
  const dialog=page.getByRole("dialog");
  await dialog.getByRole("button",{name:"País",exact:true}).click();
  await page.getByRole("menuitem",{name:"Portugal",exact:true}).click();
  await expect(dialog.getByRole("textbox",{name:"Código IBGE",exact:true})).toHaveValue("");
  await expect(dialog.getByRole("textbox",{name:"Código IBGE",exact:true})).toBeDisabled();
  const updated=page.waitForRequest(r=>r.method()==="PUT");
  await dialog.getByRole("button",{name:"Salvar",exact:true}).click();
  expect((await updated).postDataJSON()).toMatchObject({countryId:pt.id,ibgeCode:null});
});

test("validates required fields and displays server duplicate error at code",async({page})=>{
  await page.route(base,route=>route.fulfill({status:400,json:{status:400,title:"Validação",errors:{Code:["Sigla já cadastrada para este país."]}}}));
  await page.goto("/states");
  await page.getByRole("button",{name:"Novo estado",exact:true}).click();
  const dialog=page.getByRole("dialog");
  await expect(dialog.getByRole("button",{name:"Salvar",exact:true})).toBeEnabled();
  await dialog.getByRole("button",{name:"Salvar",exact:true}).click();
  await expect(page.locator("#state-country-error")).toHaveText("Selecione um país.");
  await expect(page.locator("#state-code-error")).toHaveText("Sigla deve ter entre 1 e 10 caracteres.");
  await dialog.getByRole("button",{name:"País",exact:true}).click();
  await page.getByRole("menuitem",{name:"Brasil",exact:true}).click();
  await dialog.getByRole("textbox",{name:"Estado",exact:true}).fill("São Paulo");
  await dialog.getByRole("textbox",{name:"Sigla",exact:true}).fill("SP");
  await dialog.getByRole("button",{name:"Salvar",exact:true}).click();
  await expect(page.locator("#state-code-error")).toHaveText("Sigla já cadastrada para este país.");
});

test("confirms status change and deletion",async({page})=>{
  await page.route(base+"/*",route=>route.fulfill({json:state.id}));
  await page.goto("/states");
  await page.getByRole("button",{name:"Desativar",exact:true}).click();
  const changed=page.waitForRequest(r=>r.method()==="PUT");
  await page.getByRole("dialog").getByRole("button",{name:"Confirmar",exact:true}).click();
  expect((await changed).postDataJSON()).toMatchObject({isActive:false});
  await expect(page.getByRole("dialog")).not.toBeVisible();
  await page.getByRole("button",{name:"Excluir São Paulo",exact:true}).click();
  const removed=page.waitForRequest(r=>r.method()==="DELETE");
  await page.getByRole("dialog").getByRole("button",{name:"Excluir",exact:true}).click();
  await removed;
  await expect(page.getByRole("dialog")).not.toBeVisible();
});

test("read-only user sees catalog without maintenance controls",async({page})=>{
  const perms=[...ADMIN_PERMS,permissions[0],"Permissions.Catalog.Countries.View"];
  await seedAuthedSession(page,{...TEST_USER,permissions:perms});
  await installAdminShellMocks(page,perms);
  await page.goto("/states");
  await expect(page.getByRole("cell",{name:"São Paulo",exact:true})).toBeVisible();
  await expect(page.getByRole("button",{name:"Novo estado",exact:true})).toHaveCount(0);
  await expect(page.getByRole("button",{name:"Editar São Paulo",exact:true})).toHaveCount(0);
  await expect(page.getByRole("button",{name:"Excluir São Paulo",exact:true})).toHaveCount(0);
});
