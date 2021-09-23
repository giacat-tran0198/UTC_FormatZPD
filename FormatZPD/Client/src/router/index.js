import Vue from "vue";
import VueRouter from "vue-router";
import People from "../views/People.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "People",
    component: People,
  },
  {
    path: "/knowledges",
    name: "Knowledges",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/Knowledges.vue"),
  },
  {
    path: "/zpdprofil",
    name: "ZPD",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/ZPD.vue"),
  },

];

const router = new VueRouter({
  mode: "history",
  base: "/",
  routes,
});

export default router;
