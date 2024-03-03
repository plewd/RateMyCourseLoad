import "./App.css";
import CourseSelectorList from "./components/CourseSelectorList/CourseSelectorList.tsx";
import Course from "./types.ts";

const getAllCourseOptions = () => [
  { Name: "CS2500", CreditHours: 4 },
  { Name: "CS3650", CreditHours: 4 },
  { Name: "PHIL1260", CreditHours: 4 },
  { Name: "CS2510", CreditHours: 1 },
  { Name: "BIO111", CreditHours: 4 },
];

function App() {
  // fetch all valid courses
  const courseOptions: Course[] = getAllCourseOptions();
  return (
    <>
      <h1>RateMyCourseLoad</h1>
      <CourseSelectorList courseOptions={courseOptions} />
    </>
  );
}

export default App;
