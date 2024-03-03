import Course from "../../types.ts";
import { useState } from "react";
import CourseSelector from "../CourseSelector/CourseSelector.tsx";
import { Button, Stack } from "@mui/joy";
import { Add } from "@mui/icons-material";
import DeleteIcon from '@mui/icons-material/Delete';

interface Props {
  courseOptions: Course[];
}

const maxNumberCourses = 6;

function CourseSelectorList({ courseOptions }: Props) {
  const [currentNumberCourses, setCurrentNumberCourses] = useState(4);
  // const [isAtMaxCourses, setIsAtMaxCourses] = useState(currentNumberCourses < maxNumberCourses)
  const isAtMaxCourses = currentNumberCourses >= maxNumberCourses;

  const courseElements = [];
  for (let i = 0; i < currentNumberCourses; i++) {
    courseElements.push(
      <span>
          <CourseSelector key={i} courseOptions={courseOptions} />
          {/*<DeleteIcon/>*/}
      </span>
    );
  }

  function addNewCourseSelector() {
    if (!isAtMaxCourses) {
      setCurrentNumberCourses(currentNumberCourses + 1);
    }
  }

  return (
    <>
      <Stack spacing={5}>
        {courseElements}
        <Button
          startDecorator={<Add />}
          variant={isAtMaxCourses ? "outlined" : "solid"}
          disabled={isAtMaxCourses}
          onClick={() => addNewCourseSelector()}
        >
          Add a course
        </Button>
      </Stack>
    </>
  );
}

export default CourseSelectorList;
