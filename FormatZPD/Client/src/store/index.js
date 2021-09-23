
import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

export default new Vuex.Store({
  state: { 
      isNavOpen: false, 
      student: '', 
      believes: [], 
    },
  mutations: {
      toggleNav(state) {
        state.isNavOpen = !state.isNavOpen
      }, 
      change(state, student) {
        state.student = student
      }, 
      addBelief(state, belief) {
        state.believes.push(belief)
      }
    },
  getters: {
    student: state => state.student,
    believes: state=> state.believes, 

  },
  actions: {},
  modules: {},
});


