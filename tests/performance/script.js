import http from "k6/http";
import { sleep } from "k6";

export default function () {
  http.get("http://localhost:8090/api/random/dice");
  sleep(1);
}
