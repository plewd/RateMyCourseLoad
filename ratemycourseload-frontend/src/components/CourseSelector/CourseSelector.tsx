import Course from "../../types.ts";
import { Autocomplete } from "@mui/joy";
import ClassIcon from "@mui/icons-material/Class";

interface Props {
  courseOptions: Course[];
}

function CourseSelector({ courseOptions }: Props) {
  return (
    <Autocomplete
      variant="soft"
      startDecorator={<ClassIcon />}
      options={courseOptions.map((course) => course.Name)}
    />
  );
}

export default CourseSelector;
